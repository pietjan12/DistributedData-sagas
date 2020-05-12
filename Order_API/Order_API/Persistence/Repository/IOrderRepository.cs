using Order_API.DTO;
using Order_API.Persistence.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Order_API.Repository
{
    public interface IOrderRepository
    {
        Task<DataResponseObject<Order>> GetOrderById(int id);
        Task<DataResponseObject<IEnumerable<Order>>> GetOrders();

        Task<DataResponseObject<Order>> CreateOrder(Order order);

        Task<DataResponseObject<Order>> UpdateOrderStatus(int id, Status status);
    }
}
