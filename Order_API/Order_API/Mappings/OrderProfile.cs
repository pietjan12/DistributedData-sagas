using AutoMapper;
using Order_API.DTO.Requests;
using Order_API.DTO.Response;
using Order_API.Persistence.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Order_API.Mappings
{
    public class OrderProfile : Profile
    {
        public OrderProfile()
        {
            CreateMap<Order, OrderDTO>();
            CreateMap<OrderCreateRequest, Order>();
        }
    }
}
