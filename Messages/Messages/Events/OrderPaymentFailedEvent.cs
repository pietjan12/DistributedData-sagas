using Messages.Events.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Messages.Events
{
    public class OrderPaymentFailedEvent : IEventWithIdIdentifier
    {
        public int requestID { get; }

        public OrderPaymentFailedEvent(int requestID)
        {
            this.requestID = requestID;
        }
    }
}
