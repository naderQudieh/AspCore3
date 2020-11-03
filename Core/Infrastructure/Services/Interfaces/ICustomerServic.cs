using AppZeroAPI.Entities;
using AppZeroAPI.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppZeroAPI.Services
{
    public interface ICustomerService
    {
        Task<bool> UpdateOrderStatus(string order_id, OrderStatus order_status);
        Task<bool> UpdateOrderPaymentStatus(string paymentIntentId, OrderPaymentStatus order_payment_status);
        Task<CustomerCart> GetCartForCustomer(string customer_id );
        Task<CustomerCart> GetCartDetailsForCustomer(string customer_id);
        Task<IEnumerable<CustomerOrder>> GetOrdersForCustomer(string customer_id);
        Task<CustomerCart> GetCartDetails(string cart_id );
        Task<int> AddCartDetails(CustomerCart cart );
        Task<bool> UpdateCart(CustomerCart cart);
        Task<CustomerOrder> GetOrderDetails(string order_id);
    }
}
