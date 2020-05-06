using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Order_API.Messages
{
    public class VerifyComplete
    {
        public int ID { get; }

        public VerifyComplete(int id)
        {
            ID = id;
        }
    }
}
