using Messages.Events.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Messages.Events
{
    public class OrderFailedEvent : IEventWithIdIdentifier
    {
        public int requestID { get; }

        public OrderFailedEvent(int requestID)
        {
            this.requestID = requestID;
        }
    }
}
