using AppZeroAPI.Entities;
using AppZeroAPI.Models;
using AppZeroAPI.Shared.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AppZeroAPI.Interfaces
{
    public interface IOrderRepository : IGenericRepository<AppZeroAPI.Entities.CustomerOrder>
    {
        Task<CustomerOrder> CreateOrderItemsFromCart(string cart_id);
        Task<CustomerOrder> GetOrderDetails(string order_id, bool includeItems = false);
        Task<IEnumerable<CustomerOrder>> GetOrdersForCustomer(string _customer_id);
        Task<bool> UpdateOrderStatus(string order_id, OrderStatus order_status);
        Task<bool> UpdateOrderPaymentStatus(string paymentIntentId, OrderPaymentStatus order_pymnt_status);
    }
}
