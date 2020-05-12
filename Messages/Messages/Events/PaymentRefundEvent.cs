using Messages.Events.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Messages.Events
{
    public class PaymentRefundEvent : IEventWithIdIdentifier
    {
        public int requestID { get; }
        public double RefundAmount { get; }
        public int UserId { get; }

        public PaymentRefundEvent(int requestID, double RefundAmount, int UserId)
        {
            this.requestID = requestID;
            this.RefundAmount = RefundAmount;
            this.UserId = UserId;
        }
    }
}
