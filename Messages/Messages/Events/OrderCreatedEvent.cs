using Messages.Events.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Messages.Events
{
    public class OrderCreatedEvent : IEventWithIdIdentifier
    {
        public int ID { get; }

        public OrderCreatedEvent(int id)
        {
            ID = id;
        }
    }
}
