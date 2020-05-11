using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Messages.Events;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Order_API.DTO;
using Order_API.DTO.Requests;
using Order_API.Services;
using Rebus.Bus;

namespace Order_API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost]
        public async Task<ActionResult<string>> CreateOrder([FromBody] OrderCreateRequest request)
        {
            var order  = await _orderService.CreateOrder(request);
            return new ActionResult<string>($"Order created with ID: {order.Data.Id}");
        } 
    }
}
