using Messages.Events.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Messages.Events
{
    public class OrderCreatedEvent : IEventWithIdIdentifier
    {
        public int requestID { get; }
        public double Price { get; set; }
        public int StockAmount { get; set; }
        public Guid UserId { get; set; }

        public OrderCreatedEvent(int requestID, double Price, int StockAmount, Guid UserId)
        {
            this.requestID = requestID;
            this.Price = Price;
            this.StockAmount = StockAmount;
            this.UserId = UserId;
        }
    }
}
