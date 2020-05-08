using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Messages.Events;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Order_API.DTO;
using Rebus.Bus;

namespace Order_API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly ILogger<OrderController> _logger;
        private IBus _bus;
        public OrderController(IBus bus, ILogger<OrderController> logger)
        {
            _bus = bus;
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult<string>> CreateOrder([FromBody] OrderIdRequest order)
        {
            _logger.LogInformation("Creating order with id {ID} with timeout of 10s", order.orderID);
            await _bus.Publish(new OrderCreatedEvent(order.orderID), optionalHeaders: new Dictionary<string, string> { { "optionalHeaders", "10000" } });
            return new ActionResult<string>($"Order created with ID: {order.orderID}");
        } 
    }
}
