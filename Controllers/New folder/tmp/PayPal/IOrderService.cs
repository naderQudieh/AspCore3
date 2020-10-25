using AppZeroAPI.Entities;
using AppZeroAPI.Interfaces;
using AppZeroAPI.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AppZeroAPI.Repository
{
    public interface IOrderService
    {
        Task<long> PlaceOrder(OrderRequest orderRequest);
        Task<List<Order>> GetMyOrders();
        Task<Order> GetOrderDetails();
        Task<long> PayTheOrder(OrderPaymentRequest orderPaymentRequest);
    }
}
