using AppZeroAPI.Interfaces;
using AppZeroAPI.Entities;
using Dapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Dapper.Contrib.Extensions;

namespace AppZeroAPI.Repository
{
    public class ProductRepository : BaseRepository  , IProductRepository
    {
       
        private  readonly ILogger<ProductRepository> logger;
        public ProductRepository(IConfiguration configuration, ILogger<ProductRepository> logger) : base(configuration) {
            this.logger = logger;  
        }
       
 
        public    async Task<IEnumerable<Product>> GetAllAsync()
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
                entity.AddedOn = DateTime.UtcNow;
                var result = await connection.InsertAsync(entity);
                return result;
            }
        }
        private    async Task<int> AddAsync2(Product entity)
        {
            entity.AddedOn = DateTime.Now;
            var sql = "Insert into Products (Name,Description,Barcode,Rate,AddedOn) VALUES (@Name,@Description,@Barcode,@Rate,@AddedOn)";
            using (var connection = this.GetOpenConnection())
            {  
                var result = await connection.ExecuteAsync(sql, entity);
                return result;
            }
        }


        public async Task<bool> DeleteByIdAsync(int id)
        { 
            using (var connection = this.GetOpenConnection())
            {
                var result = await connection.DeleteAsync(new Product() { Id = 1 });
                return result;
            }
        }
        private    async Task<int> DeleteByIdAsync2(int id)
        {
            var sql = "DELETE FROM Products WHERE Id = @Id";
            using (var connection = this.GetOpenConnection())
            { 
                var result = await connection.ExecuteAsync(sql, new { Id = id });
                return result;
            }
        }


        public async Task<Product> GetByIdAsync(int id)
        { 
            using (var connection = this.GetOpenConnection())
            {

                var result = await connection.GetAsync<Product>(new { id = id });
                return result;
            }
        }
        private   async Task<Product> GetByIdAsync2(int id)
        {
            var sql = "SELECT * FROM Products WHERE Id = @Id";
             using (var connection = this.GetOpenConnection())
            {
                
                var result = await connection.QuerySingleOrDefaultAsync<Product>(sql, new { Id = id });
                return result;
            }
        }

        public    async Task<int> UpdateAsync(Product entity)
        {
            entity.ModifiedOn = DateTime.Now;
            var sql = "UPDATE Products SET Name = @Name, Description = @Description, Barcode = @Barcode, Rate = @Rate, ModifiedOn = @ModifiedOn  WHERE Id = @Id";
             using (var connection = this.GetOpenConnection())
            {  
                var result = await connection.ExecuteAsync(sql, entity);
                return result;
            }
        }
    }
}
