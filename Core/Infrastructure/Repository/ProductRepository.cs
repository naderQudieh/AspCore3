using AppZeroAPI.Entities;
using AppZeroAPI.Interfaces;
using Dapper;
using Dapper.Contrib.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace AppZeroAPI.Repository
{
    public class ProductRepository : BaseRepository, IProductRepository
    {

        private readonly ILogger<ProductRepository> logger;
        public ProductRepository(IConfiguration configuration, ILogger<ProductRepository> logger) : base(configuration)
        {
            this.logger = logger;
        }


        public async Task<IEnumerable<Product>> GetAllAsync()
        {

            //logger.LogInformation(sql);
            using (var connection = this.GetOpenConnection())
            {
                var result = await connection.GetAllAsync<Product>();
                return result.ToList();
            }
        }
        private async Task<IEnumerable<Product>> GetAllAsync2()
        {

            var sql = "SELECT * FROM Products";

            logger.LogInformation(sql);
            using (var connection = this.GetOpenConnection())
            {
                var result = await connection.QueryAsync<Product>(sql);
                return result.ToList();
            }
        }

        public async Task<int> AddAsync(Product entity)
        {
            using (var connection = this.GetOpenConnection())
            {
                entity.date_created  = DateTime.UtcNow;
                entity.date_modified = DateTime.UtcNow;
                var result = await connection.InsertAsync(entity);
                return result;
            }
        }
        private async Task<int> AddAsync2(Product entity)
        {
            entity.date_created = DateTime.Now;
            entity.date_modified = DateTime.Now;
            var sql = "Insert into Products (Name,Description,Barcode,Rate,AddedOn) VALUES (@Name,@Description,@Barcode,@Rate,@AddedOn)";
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
                var result = await connection.DeleteAsync(new Product() { product_Id = 1 });
                return result;
            }
        }
        private async Task<int> DeleteByIdAsync2(long id)
        {
            var sql = "DELETE FROM Products WHERE Id = @Id";
            using (var connection = this.GetOpenConnection())
            {
                var result = await connection.ExecuteAsync(sql, new { Id = id });
                return result;
            }
        }


        public async Task<Product> GetByIdAsync(long id)
        {
            using (var connection = this.GetOpenConnection())
            {

                var result = await connection.GetAsync<Product>(new { id = id });
                return result;
            }
        }
        private async Task<Product> GetByIdAsync2(long id)
        {
            var sql = "SELECT * FROM Products WHERE Id = @Id";
            using (var connection = this.GetOpenConnection())
            {
                var result = await connection.QuerySingleOrDefaultAsync<Product>(sql, new { Id = id });
                return result;
            }
        }

        public async Task<bool> UpdateAsync(Product entity)
        {
            entity.date_modified = DateTime.Now;
            var sql = "UPDATE Products SET Name = @Name, Description = @Description, Barcode = @Barcode, Rate = @Rate, ModifiedOn = @ModifiedOn  WHERE Id = @Id";
            using (var connection = this.GetOpenConnection())
            {
                var rows = await connection.ExecuteAsync(sql, entity);
                return rows != 0;
            }
        }

       
    }
}
