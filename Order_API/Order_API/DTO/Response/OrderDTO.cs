using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Order_API.DTO.Response
{
    public class OrderDTO
    {
        public int Id { get; set; }
        public Status Status { get; set; }
    }
}
