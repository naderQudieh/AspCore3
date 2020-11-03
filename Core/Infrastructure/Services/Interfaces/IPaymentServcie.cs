using AppZeroAPI.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppZeroAPI.Services
{
    public interface IPaymentService
    {
        Task<CustomerCart> CreateOrUpdatePaymentIntent(string customerId);
        Task<bool> updateOrderPaymentfailed(string paymentId);
        Task<bool> updateOrderPaymentSucceeded(string paymentId);
    }
}
