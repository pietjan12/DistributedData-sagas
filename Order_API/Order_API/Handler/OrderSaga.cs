using Messages.Events;
using Order_API.Messages;
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
    public class OrderSaga : Saga<OrderSagaData>, IAmInitiatedBy<OrderCreatedEvent>, IAmInitiatedBy<OrderStockAvailableEvent>, IAmInitiatedBy<OrderPaymentReservedEvent>, IHandleMessages<VerifyComplete>
    {
        static readonly ILogger Logger = Log.ForContext<OrderSaga>();

        readonly IBus _bus;

        public OrderSaga (IBus bus)
        {
            _bus = bus;
        }

        protected override void CorrelateMessages(ICorrelationConfig<OrderSagaData> config)
        {
            // Events welke we willen volgen in deze saga.
            config.Correlate<OrderCreatedEvent>(m => m.requestID, d => d.requestID);
            config.Correlate<OrderStockAvailableEvent>(m => m.requestID, d => d.requestID);
            config.Correlate<OrderPaymentReservedEvent>(m => m.requestID, d => d.requestID);

            // interne verificatie achteraf.
            config.Correlate<VerifyComplete>(m => m.requestID, d => d.requestID);
        }

        public async Task Handle(OrderCreatedEvent message)
        {
            await Pre();

            Logger.Information("Setting {FieldName} to true for order {ID}", "Order Created", Data.requestID);

            Data.OrderCreated = true;

            await Post();
        }

        public async Task Handle(OrderStockAvailableEvent message)
        {
            await Pre();

            Logger.Information("Setting {FieldName} to true for order {ID}", "Order Stock Available and reserved", Data.requestID);
            Data.OrderStockAvailable = true;

            await Post();
        }

        public async Task Handle(OrderPaymentReservedEvent message)
        {
            await Pre();

            Logger.Information("Setting {FieldName} to true for order {ID}", "Payment Reserved", Data.requestID);
            Data.OrderPaymentReserved = true;
            await Post();
        }

        public async Task Handle(VerifyComplete message)
        {
            Logger.Warning("The saga for order {ID} was not completed within {TimeoutSeconds} s timeout", Data.requestID, 20);

            await _bus.Publish(new OrderFailedEvent(Data.requestID));

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

            Logger.Information("Publishing ready event and marking saga for order {ID} as complete", Data.requestID);

            await _bus.Publish(new OrderReadyEvent(Data.requestID));

            MarkAsComplete();
        }
    }

    public class OrderSagaData : SagaData
    {
        public int requestID { get; set; }

        public bool OrderCreated { get; set; }
        public bool OrderStockAvailable { get; set; }
        public bool OrderPaymentReserved { get; set; }

        public bool IsDone => OrderCreated
                              && OrderStockAvailable
                              && OrderPaymentReserved;
    }
}
