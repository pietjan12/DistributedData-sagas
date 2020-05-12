using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Messages.Events;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Order_API.DTO;
using Order_API.DTO.Requests;
using Order_API.DTO.Response;
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
        public async Task<ActionResult<OrderDTO>> CreateOrder([FromBody] OrderCreateRequest request)
        {
            var order  = await _orderService.CreateOrder(request);
            return CreatedAtAction(nameof(GetOrderById), new { id = order.Data.Id }, order.Data);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<OrderDTO>> GetOrderById(int id)
        {
            var order = await _orderService.GetOrderById(id);
            if(!order.Success) {
                return NotFound(order.Message);
            }
            return Ok(order.Data);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderDTO>>> GetOrders()
        {
            var orders = await _orderService.GetOrders();
            return Ok(orders.Data);
        }
    }
}
