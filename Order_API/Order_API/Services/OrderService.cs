using AutoMapper;
using Messages.Events;
using Microsoft.Extensions.Logging;
using Order_API.DTO;
using Order_API.DTO.Requests;
using Order_API.DTO.Response;
using Order_API.Persistence.Entities;
using Order_API.Repository;
using Rebus.Bus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Order_API.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IBus _bus;
        private readonly IMapper _mapper;
        private readonly ILogger<OrderService> _logger;
        public OrderService(IOrderRepository orderRepository, IMapper mapper, IBus bus, ILogger<OrderService> logger)
        {
            _orderRepository = orderRepository;
            _mapper = mapper;
            _bus = bus;
            _logger = logger;
        }

        public async Task<DataResponseObject<OrderDTO>> CreateOrder(OrderCreateRequest order)
        {
            var createdOrder = await _orderRepository.CreateOrder(_mapper.Map<Order>(order));
            if(createdOrder.Success)
            {
                _logger.LogInformation("Creating order with id {ID} with timeout of 10s", createdOrder.Data.Id);
                await _bus.Publish(new OrderCreatedEvent(createdOrder.Data.Id), optionalHeaders: new Dictionary<string, string> { { "x-message-ttl", "10000" } });
            }
            return _mapper.Map<DataResponseObject<OrderDTO>>(createdOrder.Data);
        }

        public async Task<DataResponseObject<OrderDTO>> GetOrderById(int id)
        {
            var order = await _orderRepository.GetOrderById(id);
            return _mapper.Map<DataResponseObject<OrderDTO>>(order.Data);
        }

        public async Task<DataResponseObject<IEnumerable<OrderDTO>>> GetOrders()
        {
            var orders = await _orderRepository.GetOrders();
            return _mapper.Map<DataResponseObject<IEnumerable<OrderDTO>>>(orders);

        }
    }
}
