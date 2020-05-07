using Messages.Events.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Messages.Events
{
    public class OrderCreatedEvent : IEventWithIdIdentifier
    {
        public int requestID { get; }

        public OrderCreatedEvent(int requestID)
        {
            this.requestID = requestID;
        }
    }
}
