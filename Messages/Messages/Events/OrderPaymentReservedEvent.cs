using Messages.Events.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Messages.Events
{
    public class OrderPaymentReservedEvent : IEventWithIdIdentifier
    {
        public int requestID { get; }

        public OrderPaymentReservedEvent(int requestID)
        {
            this.requestID = requestID;
        }
    }
}
