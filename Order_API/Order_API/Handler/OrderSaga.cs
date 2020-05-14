using Messages.Events;
using Order_API.Messages;
using Order_API.Services;
using Rebus.Bus;
using Rebus.Handlers;
using Rebus.Sagas;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Order_API.Handler
{
    public class OrderSaga : Saga<OrderSagaData>, IAmInitiatedBy<OrderCreatedEvent>, IAmInitiatedBy<OrderStockAvailableEvent>, IAmInitiatedBy<OrderPaymentReservedEvent>, IAmInitiatedBy<OrderStockNotAvailableEvent>, IAmInitiatedBy<OrderPaymentFailedEvent>, IHandleMessages<PaymentRefundedEvent> ,IHandleMessages<VerifyComplete>
    {
        static readonly ILogger Logger = Log.ForContext<OrderSaga>();

        readonly IBus _bus;
        readonly IOrderService _orderService;

        public OrderSaga (IBus bus, IOrderService orderService)
        {
            _bus = bus;
            _orderService = orderService;
        }

        protected override void CorrelateMessages(ICorrelationConfig<OrderSagaData> config)
        {
            // Link all the events with given ID which we want to follow.
            config.Correlate<OrderCreatedEvent>(m => m.requestID, d => d.requestID);
            config.Correlate<OrderStockAvailableEvent>(m => m.requestID, d => d.requestID);
            config.Correlate<OrderPaymentReservedEvent>(m => m.requestID, d => d.requestID);
            config.Correlate<OrderStockNotAvailableEvent>(m => m.requestID, d => d.requestID);
            config.Correlate<OrderPaymentFailedEvent>(m => m.requestID, d => d.requestID);

            //Rollback events to listen for in this saga
            config.Correlate<PaymentRefundedEvent>(m => m.requestID, d => d.requestID);

            // Link timeout event to saga so we can gracefully timeout a saga and rollback.
            config.Correlate<VerifyComplete>(m => m.requestID, d => d.requestID);
        }

        public async Task Handle(OrderCreatedEvent message)
        {
            await Pre();

            Logger.Information("Setting {FieldName} to true for order {ID}", "Order Created", Data.requestID);
            Data.OrderCreated = true;

            //set saga data for initiating future events, based on the order created event message
            Data.Price = message.Price;
            Data.StockAmount = message.StockAmount;
            Data.UserId = message.UserId;

            //publish the event that the payment service subscribes to with the necessary data mocked
            Logger.Information($"Publishing PaymentReserveEvent event for order {message.requestID}");
            await _bus.Publish(new PaymentReserveEvent(Data.requestID, Data.Price, Data.UserId), optionalHeaders: new Dictionary<string, string> { { "x-message-ttl", "10000" } });

            await Post();
        }

        public async Task Handle(OrderStockAvailableEvent message)
        {
            await Pre();

            Logger.Information("Setting {FieldName} to true for order {ID}", "Order Stock Available and reserved", Data.requestID);
            Data.OrderStockAvailable = true;

            //update order status
            await _orderService.UpdateOrderStatus(message.requestID, Status.STOCK_RESERVED);

            await Post();
        }

        public async Task Handle(OrderPaymentReservedEvent message)
        {
            await Pre();

            Logger.Information("Setting {FieldName} to true for order {ID}", "Payment Reserved", Data.requestID);
            Data.OrderPaymentReserved = true;

            //update order status
            await _orderService.UpdateOrderStatus(message.requestID, Status.PAYMENT_RESERVED);

            //publish the event to reserve the stock since payment went through
            Logger.Information($"Publishing StockReserveEvent event for order {message.requestID}");
            await _bus.Publish(new StockReserveEvent(message.requestID, Data.StockAmount), optionalHeaders: new Dictionary<string, string> { { "x-message-ttl", "10000" } });

            await Post();
        }

        public async Task Handle(OrderStockNotAvailableEvent message)
        {
            await Pre();

            Logger.Warning("Stock unavailable for order ID: {id}", message.requestID);

            Data.OrderStockUnavailable = true;

            await Post();
        }

        public async Task Handle(OrderPaymentFailedEvent message)
        {
            await Pre();

            Logger.Warning("Payment unavailable for order ID: {id}", message.requestID);

            Data.OrderPaymentFailed = true;

            await Post();
        }

        public async Task Handle(PaymentRefundedEvent message)
        {
            //payment has been refunded and rollback is complete, Saga can now be marked as complete since this is the last rollback step.
            await Task.Run(() => MarkAsComplete());
        }

        public async Task Handle(VerifyComplete message)
        {
            Logger.Warning("The saga for order {ID} was not completed within {TimeoutSeconds} s timeout", Data.requestID, 20);

            //timeout occured, initiate rollback
            await _bus.Publish(new OrderFailedEvent(Data.requestID));

            await RollBack(true);

            //should probably be moved to rollback or paymentrefunded event logic.
            MarkAsComplete();
        }

        async Task Pre()
        {
            if (!IsNew) return;

            Logger.Information("Ordering wake-up call in {second} s  for order {ID}", 20, Data.requestID);

            await _bus.DeferLocal(TimeSpan.FromSeconds(20), new VerifyComplete(Data.requestID));
        }

        async Task Post()
        {
            if (!Data.IsDone) return;

            if (Data.IsError)
            {
                //update order status
                await _orderService.UpdateOrderStatus(Data.requestID, Status.FAILED);

                //error occured, initiate rollback.
                await RollBack();
            }
            else
            {
                //No error occured, publish order ready event.
                Logger.Information("Publishing ready event and marking saga for order {ID} as complete", Data.requestID);

                //update order status
                await _orderService.UpdateOrderStatus(Data.requestID, Status.COMPLETE);

                await _bus.Publish(new OrderReadyEvent(Data.requestID));

                //No rollback needed and order ready event has been published, mark saga as complete
                MarkAsComplete();
            }
        }

        async Task RollBack(bool cancelled = false)
        {
            if (cancelled)
            {

            }
            else
            {
                //wasnt cancelled by timeout, check for errors
                if (Data.IsError)
                {
                    if(Data.OrderPaymentFailed)
                    {
                        Logger.Information("Order payment failed which does not require any additional rollbacks up the chain, closing saga...");
                        //payment failed is first step in the saga and we dont need to do any rollback actions this way. so we mark the saga as complete
                        MarkAsComplete();
                    }

                    if (Data.OrderStockUnavailable)
                    {
                        Logger.Information("Stock was not available, initialising payment refund procedure....");
                        //publish event with no expiration time. to initiate refund, pull price from sagadata stored at creation of saga.
                        await _bus.Publish(new PaymentRefundEvent(Data.requestID, Data.Price, Data.UserId));
                    }
                }
            }
        }
    }

    public class OrderSagaData : SagaData
    {
        public int requestID { get; set; }
        public double Price { get; set; }
        public int StockAmount { get; set; }
        public Guid UserId { get; set; }

        public bool OrderCreated { get; set; }
        public bool OrderStockAvailable { get; set; }
        public bool OrderPaymentReserved { get; set; }

        public bool OrderStockUnavailable { get; set; }
        public bool OrderPaymentFailed { get; set; }

        public bool IsDone => OrderCreated
                              && OrderStockAvailable
                              && OrderPaymentReserved
                              || OrderCreated 
                              && (OrderStockUnavailable || OrderPaymentFailed);

        public bool IsError => OrderStockUnavailable || OrderPaymentFailed;

    }
}
