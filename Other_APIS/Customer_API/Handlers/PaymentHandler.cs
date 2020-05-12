using Messages.Events;
using Rebus.Bus;
using Rebus.Handlers;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Other_APIS.Handlers
{
    public class PaymentHandler : IHandleMessages<PaymentReserveEvent>, IHandleMessages<PaymentRefundEvent>
    {
        readonly ILogger Logger = Log.ForContext<PaymentHandler>();
        readonly IBus _bus;

        public PaymentHandler(IBus bus)
        {
            _bus = bus;
        }

        public async Task Handle(PaymentReserveEvent message)
        {
            Logger.Information($"PaymentReserveEvent received for id: {message.requestID} with price: {message.Price} for user: {message.UserId}");

            if(message.Price >= 50)
            {
                Logger.Information($"Publishing OrderPaymentFailedEvent for order : {message.requestID}");
                //send fail event, not enough credit for user
                await _bus.Publish(new OrderPaymentFailedEvent(message.requestID));
            } else
            {
                Logger.Information($"Publishing OrderPaymentReservedEvent for order : {message.requestID}");
                //he has the money, reserve it(theoretically) and then send event
                await _bus.Publish(new OrderPaymentReservedEvent(message.requestID));
            }
        }

        public async Task Handle(PaymentRefundEvent message)
        {
            Logger.Information($"PaymentRefundEvent received for id: {message.requestID} with refund amount: {message.RefundAmount} for user: {message.UserId}");

            //handle refund logic here theoretically

            //publish event that we have refunded the user
            Logger.Information($"Publishing PaymentRefundedEvent for order : {message.requestID}");
            await _bus.Publish(new PaymentRefundedEvent(message.requestID));
            
        }
    }
}
