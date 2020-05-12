using Messages.Events.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Messages.Events
{
    public class StockReserveEvent : IEventWithIdIdentifier
    {
        public int requestID { get; }
        public int AmountOfStock { get; }
        public StockReserveEvent(int requestID, int amountOfStock)
        {
            this.requestID = requestID;
            this.AmountOfStock = amountOfStock;
        }
    }
}
