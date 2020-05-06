using Messages.Events.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Messages.Events
{
    public class OrderStockAvailableEvent : IEventWithIdIdentifier
    {
        public int ID { get; }

        public OrderStockAvailableEvent(int id)
        {
            ID = id;
        }
    }
}
