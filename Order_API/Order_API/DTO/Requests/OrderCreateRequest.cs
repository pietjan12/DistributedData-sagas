using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Order_API.DTO.Requests
{
    public class OrderCreateRequest
    {
        public Guid UserId { get; set; }
        public double Price { get; set; }
        public int StockAmount { get; set; }
    }
}
