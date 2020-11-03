using AppZeroAPI.Entities;
using AppZeroAPI.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppZeroAPI.Services
{
    public interface IPaymentService
    {
        Task<CustomerCart> CreateOrUpdatePaymentIntent(string customerId);
        Task<bool> UpdateOrderStatus(string order_id, OrderStatus order_status);
        Task<bool> UpdateOrderPaymentStatus(string paymentIntentId, OrderPaymentStatus order_payment_status);
    }
}
