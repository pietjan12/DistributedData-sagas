using Messages.Events.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Messages.Events
{
    public class PaymentReserveEvent : IEventWithIdIdentifier
    {
        public int requestID { get; }
        public double Price { get; }
        public int UserId { get; }

        public PaymentReserveEvent(int requestID, double Price, int UserId)
        {
            this.requestID = requestID;
            this.Price = Price;
            this.UserId = UserId;
        }
    }
}
