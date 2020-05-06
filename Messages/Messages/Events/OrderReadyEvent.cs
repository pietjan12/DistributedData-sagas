using Messages.Events.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Messages.Events
{
    public class OrderReadyEvent : IEventWithIdIdentifier
    {
        public int ID { get; }

        public OrderReadyEvent(int id)
        {
            ID = id;
        }
    }
}
