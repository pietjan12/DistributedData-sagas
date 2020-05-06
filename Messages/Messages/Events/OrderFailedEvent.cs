using Messages.Events.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Messages.Events
{
    public class OrderFailedEvent : IEventWithIdIdentifier
    {
        public int ID { get; }

        public OrderFailedEvent(int id)
        {
            ID = id;
        }
    }
}
