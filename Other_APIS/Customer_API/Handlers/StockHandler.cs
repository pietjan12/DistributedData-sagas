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
    public class StockHandler : IHandleMessages<StockReserveEvent>
    {
        readonly ILogger Logger = Log.ForContext<PaymentHandler>();
        readonly IBus _bus;

        public StockHandler(IBus bus)
        {
            _bus = bus;
        }

        public async Task Handle(StockReserveEvent message)
        {

            Logger.Information($"StockReserveEvent received for id: {message.requestID} with stock: {message.AmountOfStock}");
            if(message.AmountOfStock >= 50)
            {
                Logger.Information($"Publishing OrderStockNotAvailableEvent for order : {message.requestID}");
                await _bus.Publish(new OrderStockNotAvailableEvent(message.requestID));
            } else
            {
                Logger.Information($"Publishing OrderStockAvailableEvent for order : {message.requestID}");
                //we have enough stock, reserve it(theoretically) then send success event
                await _bus.Publish(new OrderStockAvailableEvent(message.requestID));
            }
        }
    }
}
