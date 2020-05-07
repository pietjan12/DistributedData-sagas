using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Order_API.Messages
{
    public class VerifyComplete
    {
        public int requestID { get; }

        public VerifyComplete(int requestID)
        {
            this.requestID = requestID;
        }
    }
}
