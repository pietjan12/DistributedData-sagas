using Messages.Events.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Messages.Events
{
    public class OrderStockAvailableEvent : IEventWithIdIdentifier
    {
        public int requestID { get; }

        public OrderStockAvailableEvent(int requestID)
        {
            this.requestID = requestID;
        }
    }
}
