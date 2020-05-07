using System;
using System.Collections.Generic;
using System.Text;

namespace Messages.Events.Interfaces
{
    public interface IEventWithIdIdentifier
    {
        int requestID { get; }
    }
}
