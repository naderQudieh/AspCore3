using AppZeroAPI.Entities;
using AppZeroAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace AppZeroAPI.Interfaces
{
    public interface IPaymentRepository   
    {
        Task<bool> UpdatePaymentStatus(string payment_id, string payment_status);
        Task<IEnumerable<Payment>> CreatePayment(IEnumerable<Payment> payment);


        Task<IEnumerable<Payment>> GetCustomerPayments(long customer_id);
       
        Task<Payment> GetCustomerPayment(long customer_id, string payment_id);

        Task<IEnumerable<CustomerCreditCard>> GetCustomerCreditCards(long customer_id);

        Task<int> AddCustomerCard(CustomerCreditCard entity);

        Task<bool> DeleteCustomerCreditCard(string card_id);
    }
}
