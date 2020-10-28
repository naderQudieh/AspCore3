using AppZeroAPI.Entities;
using AppZeroAPI.Interfaces;
using Dapper;
using Dapper.Contrib.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AppZeroAPI.Models;
using System.Linq.Expressions;
using AppZeroAPI.Services;
namespace AppZeroAPI.Repository
{

    public class PaymentRepository : BaseRepository, IPaymentRepository
    {
        
        private readonly BraintreeService braintreeService;
        private readonly ILogger<PaymentRepository> logger;
        public PaymentRepository(BraintreeService braintreeService , IConfiguration configuration, ILogger<PaymentRepository> logger) : base(configuration)
        {
            this.braintreeService = braintreeService;
            this.logger = logger;
        }
         
        public async Task<IEnumerable<Payment>> CreatePayment(IEnumerable<Payment> payment)
        {
            //logger.LogInformation(sql);
           // braintreeService.Sale();
            using (var connection = this.GetOpenConnection())
            {
                var result = await connection.GetAllAsync<Payment>();
                return result.ToList();
            }
        }
        public async Task<Payment> GetCustomerPayment(long customer_id, string payment_id)
        {
            var sql = "select  * from customer_payments WHERE customer_id = @customer_id and payment_id = @payment_id";
            using (var connection = this.GetOpenConnection())
            {
                var result = await connection.QueryFirstOrDefault(sql, new { customer_id = customer_id, payment_id = payment_id });
                return result;
            }
        }
        public async Task<Payment> GetCustomerPayment(long payment_id)
        {
            var sql = "select  * from customer_payments WHERE payment_id = @payment_id";
            using (var connection = this.GetOpenConnection())
            { 
                var result = await connection.QueryFirstOrDefault(sql, new { payment_id = payment_id });
                return result;
            }
        }
        public async Task<IEnumerable<Payment>> GetOrderPayment(long order_id)
        {
            var sql = "select  * from customer_payments WHERE order_id= @order_id ";
            using (var connection = this.GetOpenConnection())
            {
                var result = await connection.QueryAsync<Payment>(sql, new {  order_id = order_id });
                return result;
            }
        }
        public async Task<IEnumerable<Payment>> GetCustomerPayments(long customer_id)
        {
            var sql = "select * FROM customer_payments WHERE customer_id = @customer_id";
            using (var connection = this.GetOpenConnection())
            {

                var result = await connection.QueryAsync<Payment>(sql, new { customer_id = customer_id });
                return result;
            }
        }
        public async Task<IEnumerable<Payment>> GetPaymentsOfcard(long last_4_digist)
        {
            var sql = "select * FROM customer_payments WHERE card_number like @last_4_digist";
            using (var connection = this.GetOpenConnection())
            {

                var result = await connection.QueryAsync<Payment>(sql, new { last_4_digist = last_4_digist });
                return result;
            } 
        }
        public async Task<int> AddCustomerCard(CustomerCreditCard entity)
        {
            entity.card_id = Guid.NewGuid().ToString().Replace("-", "").ToLower();
            entity.date_created = DateTime.UtcNow;
            entity.date_modified = DateTime.UtcNow;
            var sql = @"Insert into customer_cards (  [card_id], [customer_id]
                               ,[card_number]      ,[card_holder_name]
                          ,[card_type]   ,[card_exp_mm]      ,[card_exp_yy]
                          ,[card_cvv]   ,[card_status]      ,[date_created]
                          ,[date_modified])
            VALUES (@customer_id,@card_id,@card_number,@card_holder_name,
            @card_type,@card_exp_mm,@card_exp_yy,@card_cvv,@card_status,@card_status,@date_created,@date_modified   )";
            using (var connection = this.GetOpenConnection())
            {
                var result = await connection.ExecuteAsync(sql, entity);
                return result;
            }
        }

        public async Task<bool> UpdateAsync(CustomerCreditCard entity)
        {
            entity.date_modified = DateTime.UtcNow;
            var sql = @"UPDATE customer_cards SET card_type = @card_type, date_modified = @date_modified, card_exp_mm = @card_exp_mm, 
             card_exp_yy = @card_exp_yy, card_cvv = @card_cvv, card_status = @card_status  WHERE card_id = @card_id";
            using (var connection = this.GetOpenConnection())
            {
                int rows  = await connection.ExecuteAsync(sql, entity);
                return rows != 0;
            }
        }

        public async Task<IEnumerable<CustomerCreditCard>> GetCustomerCreditCards(long customer_id)
        {
            var sql = "select * FROM customer_cards WHERE customer_id = @customer_id";
            using (var connection = this.GetOpenConnection())
            {

                var result = await connection.QueryAsync<CustomerCreditCard>(sql, new { customer_id = customer_id });
                return result;
            }
        }
        public async Task<bool> DeleteCustomerCreditCard(string card_id)
        {
            using (var connection = this.GetOpenConnection())
            {
                var result = await connection.DeleteAsync(new CustomerCreditCard() { card_id = card_id });
                return result;
            }
        }

        
    }
}

