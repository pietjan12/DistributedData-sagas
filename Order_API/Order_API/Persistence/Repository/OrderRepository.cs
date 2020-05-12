using Microsoft.EntityFrameworkCore;
using Order_API.DTO;
using Order_API.Persistence.Context;
using Order_API.Persistence.Entities;
using Order_API.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Order_API.Persistence.Repository
{
    public class OrderRepository : IOrderRepository
    {
        private readonly OrderDbContext _context;

        public OrderRepository(OrderDbContext context)
        {
            _context = context;
        }

        public async Task<DataResponseObject<Order>> CreateOrder(Order order)
        {
            order.Status = Status.PENDING;
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            return new DataResponseObject<Order>(order);
        }

        public async Task<DataResponseObject<Order>> GetOrderById(int id)
        {
            Order foundOrder = await _context.Orders.FindAsync(id);
            if (foundOrder == null)
            {
                //return server response object
                return new DataResponseObject<Order>("Order could not be found");
            }
            return new DataResponseObject<Order>(foundOrder);
        }

        public async Task<DataResponseObject<IEnumerable<Order>>> GetOrders()
        {
            var orders = await _context.Orders.ToListAsync();
            return new DataResponseObject<IEnumerable<Order>>(orders);
        }

        public async Task<DataResponseObject<Order>> UpdateOrderStatus(int id, Status status)
        {
            var order = await _context.Orders.FindAsync(id);
            if(order == null)
            {
                return new DataResponseObject<Order>("Order could not be found");
            }
            order.Status = status;
            await _context.SaveChangesAsync();
            return new DataResponseObject<Order>(order);
        }
    }
}
