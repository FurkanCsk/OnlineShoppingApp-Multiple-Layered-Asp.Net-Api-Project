﻿using OnlineShoppingApp.Business.Operations.Order.Dtos;
using OnlineShoppingApp.Business.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShoppingApp.Business.Operations.Order
{
    public interface IOrderService
    {
        Task<OrderDto> GetOrder(int id);
        Task<List<OrderDto>> GetOrders();
        Task<ServiceMessage> AddOrder(AddOrderDto order);
        Task<ServiceMessage> DeleteOrder(int id);
        Task<ServiceMessage> UpdateOrder(UpdateOrderDto order);
    }
}
