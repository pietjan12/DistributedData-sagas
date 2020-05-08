using Messages.Events.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Messages.Events
{
    public class OrderStockNotAvailableEvent : IEventWithIdIdentifier
    {
        public int requestID { get; }

        public OrderStockNotAvailableEvent(int requestID)
        {
            this.requestID = requestID;
        }
    }
}
