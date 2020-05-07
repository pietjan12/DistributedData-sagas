using Messages.Events.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Messages.Events
{
    public class OrderReadyEvent : IEventWithIdIdentifier
    {
        public int requestID { get; }

        public OrderReadyEvent(int requestID)
        {
            this.requestID = requestID;
        }
    }
}
