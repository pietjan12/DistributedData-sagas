using Order_API.DTO;
using Order_API.DTO.Requests;
using Order_API.DTO.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Order_API.Services
{
    public interface IOrderService
    {
        Task<DataResponseObject<OrderDTO>> GetOrderById(int id);
        Task<DataResponseObject<IEnumerable<OrderDTO>>> GetOrders();
        Task<DataResponseObject<OrderDTO>> CreateOrder(OrderCreateRequest order);
    }
}
