using AppZeroAPI.Entities;
using AppZeroAPI.Models;
using AppZeroAPI.Interfaces;
using Dapper;
using Dapper.Contrib.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppZeroAPI.Repository
{
    public class CustomerRepository : BaseRepository, ICustomerRepository
    {

        public readonly ILogger<CustomerRepository> logger;
        public CustomerRepository(IConfiguration configuration, ILogger<CustomerRepository> logger) : base(configuration)
        {
            this.logger = logger;
        }

        public async Task<IEnumerable<Customer>> GetAllAsync()
        {

            //logger.LogInformation(sql);
            using (var connection = this.GetOpenConnection())
            {
                var result = await connection.GetAllAsync<Customer>();
                return result.ToList();
            }
        }
        public async Task<Customer> GetByIdAsync(string rec_id)
        {
            using (var connection = this.GetOpenConnection())
            {

                var result = await connection.GetAsync<Customer>(new { rec_id = rec_id });
                return result;
            }
        }
        public async Task<Customer> GetByIdAsync(string customrid, string cart_id)
        {
            using (var connection = this.GetOpenConnection())
            {

                var result = await connection.GetAsync<Customer>(new {  cart_id = cart_id });
                return result;
            }
        }
        public async Task<int> AddAsync(Customer entity)
        {
            using (var connection = this.GetOpenConnection())
            {
                //entity.AddedOn = DateTime.UtcNow;
                var result = await connection.InsertAsync(entity);
                return result;
            }
        }

        public async Task<bool> UpdateAsync(Customer entity)
        {
            using (var connection = this.GetOpenConnection())
            {
                var isSuucess = await connection.UpdateAsync(entity);
                return isSuucess;
            }

        }

        public async Task<bool> DeleteByIdAsync(string rec_id)
        {
            using (var connection = this.GetOpenConnection())
            {
                var result = await connection.DeleteAsync(new Customer() { rec_id = rec_id });
                return result;
            }
        }
         
    
    }
}
