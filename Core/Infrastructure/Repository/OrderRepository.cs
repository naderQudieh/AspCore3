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
    public class OrderRepository : BaseRepository, IOrderRepository
    {

        private readonly ILogger<OrderRepository> logger;
        public OrderRepository(IConfiguration configuration, ILogger<OrderRepository> logger) : base(configuration)
        {
            this.logger = logger;
        }


        public async Task<IEnumerable<Order>> GetAllAsync()
        {

            //logger.LogInformation(sql);
            using (var connection = this.GetOpenConnection())
            {
                var result = await connection.GetAllAsync<Order>();
                return result.ToList();
            }
        }
        private async Task<IEnumerable<Order>> GetAllAsync2()
        {

            var sql = "SELECT * FROM Orders";

            logger.LogInformation(sql);
            using (var connection = this.GetOpenConnection())
            {
                var result = await connection.QueryAsync<Order>(sql);
                return result.ToList();
            }
        }

        public async Task<int> AddAsync(Order entity)
        {
            entity.date_created = DateTime.UtcNow;
            entity.date_modified = DateTime.UtcNow;
            using (var connection = this.GetOpenConnection())
            {
                // entity.AddedOn = DateTime.UtcNow;
                var result = await connection.InsertAsync(entity);
                return result;
            }
        }
        private async Task<int> AddAsync2(Order entity)
        {
            entity.date_created = DateTime.UtcNow;
            entity.date_modified = DateTime.UtcNow;
            var sql = @"Insert into Orders ( ,name
                        ,description,barcode,qty_in_stock ,unit_price ,imge_url
                        ,department_id,date_created,date_modified
                        ) VALUES (
                            @name,@description  ,@barcode  ,@qty_in_stock ,@unit_price 
                            ,@imge_url ,@department_id ,@date_created ,@date_modified;)";
            using (var connection = this.GetOpenConnection())
            {
                var result = await connection.ExecuteAsync(sql, entity);
                return result;
            }
        }


        public async Task<bool> DeleteByIdAsync(long id)
        {
            using (var connection = this.GetOpenConnection())
            {
                var result = await connection.DeleteAsync(new Order() { order_id = 1 });
                return result;
            }
        }
        private async Task<int> DeleteByIdAsync2(long id)
        {
            var sql = "DELETE FROM Orders WHERE Id = @Id";
            using (var connection = this.GetOpenConnection())
            {
                var result = await connection.ExecuteAsync(sql, new { Id = id });
                return result;
            }
        }


        public async Task<Order> GetByIdAsync(long id)
        {
            using (var connection = this.GetOpenConnection())
            {

                var result = await connection.GetAsync<Order>(new { id = id });
                return result;
            }
        }
        private async Task<Order> GetByIdAsync2(long id)
        {
            var sql = "SELECT * FROM Orders WHERE Id = @Id";
            using (var connection = this.GetOpenConnection())
            {

                var result = await connection.QuerySingleOrDefaultAsync<Order>(sql, new { Id = id });
                return result;
            }
        }

        public async Task<bool> UpdateAsync(Order entity)
        {
           // entity.ModifiedOn = DateTime.UtcNow;
            var sql = "UPDATE Orders SET Name = @Name, Description = @Description, Barcode = @Barcode, Rate = @Rate, ModifiedOn = @ModifiedOn  WHERE Id = @Id";
            using (var connection = this.GetOpenConnection())
            {
                var rows = await connection.ExecuteAsync(sql, entity);
                return rows != 0;
            }
        }
    }
}
