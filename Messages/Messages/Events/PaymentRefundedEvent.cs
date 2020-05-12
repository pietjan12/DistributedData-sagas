using Messages.Events.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Messages.Events
{
    public class PaymentRefundedEvent : IEventWithIdIdentifier
    {
        public int requestID { get; }

        public PaymentRefundedEvent(int requestID)
        {
            this.requestID = requestID;
        }
    }
}
