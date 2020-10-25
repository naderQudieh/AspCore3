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
        public async Task<Customer> GetByIdAsync(long customrid)
        {
            using (var connection = this.GetOpenConnection())
            {

                var result = await connection.GetAsync<Customer>(new { customer_id = customrid });
                return result;
            }
        }
        public async Task<Customer> GetByIdAsync(long customrid, long cart_id)
        {
            using (var connection = this.GetOpenConnection())
            {

                var result = await connection.GetAsync<Customer>(new { cart_id = cart_id, customer_id = customrid });
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


      
        public async Task<bool> DeleteByIdAsync(long id)
        {
            using (var connection = this.GetOpenConnection())
            {
                var result = await connection.DeleteAsync(new Customer() { customer_id = 1 });
                return result;
            }
        }

        public async Task<int> DeleteCustomerCartByIdAsync(long customer_id, long cart_id)
        {
            var sql = "delete from customer_carts WHERE cart_id = @cart_id";
            using (var connection = this.GetOpenConnection())
            {
                var result = await connection.ExecuteAsync(sql, new { cart_id = cart_id });
                return result;
            }
        }
       
        public async Task<bool> UpdateAsync(Customer entity)
        {
            // entity.ModifiedOn = DateTime.UtcNow;
            var sql = "UPDATE Orders SET Name = @Name, Description = @Description, Barcode = @Barcode, Rate = @Rate, ModifiedOn = @ModifiedOn  WHERE Id = @Id";
            using (var connection = this.GetOpenConnection())
            {
                var rows = await connection.ExecuteAsync(sql, entity);
                return rows !=0 ;
            }
        }


        public async Task<int> AddCart(CustomerCart entity)
        {
            entity.cart_id = Guid.NewGuid().ToString().Replace("-", "").ToLower();
            entity.date_created = DateTime.UtcNow;
            entity.date_modified = DateTime.UtcNow;
            using (var connection = this.GetOpenConnection())
            {
                //entity.AddedOn = DateTime.UtcNow;
                var result = await connection.InsertAsync(entity);
                return result;
            }
        }
        public async Task<int> AddCartItem(CartItem entity)
        {
            entity.cart_id = Guid.NewGuid().ToString().Replace("-", "").ToLower();
            entity.date_created = DateTime.UtcNow;
            entity.date_modified = DateTime.UtcNow;
            using (var connection = this.GetOpenConnection())
            { 
                var result = await connection.InsertAsync(entity);
                return result;
            }
        }
        
        public async Task<bool> UpdateCartItem(CartItem entity)
        {
            var sql = @"UPDATE customer_carts_items
                             SET product_id = @product_id,
                                 cart_id = @cart_id, 
                                 qty = @qty
                             WHERE Id = @Id";
            using (var connection = this.GetOpenConnection())
            {
                var rows = await connection.ExecuteAsync(sql,entity);
                return rows != 0;
            }
            
        }


        public async Task<int> DeleteCartItem(long cart_id, long cart_item_id)
        {
            var sql = "delete from customer_carts_items  WHERE cart_id = @cart_id and  cart_item_id=@cart_item_id ";
            using (var connection = this.GetOpenConnection())
            {
                var result = await connection.ExecuteAsync(sql, new { cart_id = cart_id, cart_item_id = cart_item_id });
                return result;
            }
        }
        public async Task<int> DeleteCartByCustomerId(long customer_id)
        {
            var sql = "delete from customer_cart  WHERE customer_id = @customer_id;   ";
            using (var connection = this.GetOpenConnection())
            {
                var result = await connection.ExecuteAsync(sql, new { customer_id = customer_id });
                return result;
            }
        }
        public async Task<int> DeleteCartById(long cart_id)
        {
            var sql = "delete from customer_carts WHERE cart_id = @cart_id; ";
            using (var connection = this.GetOpenConnection())
            {
                var result = await connection.ExecuteAsync(sql, new { cart_id = cart_id });
                return result;
            }
        }
        public async Task<int> DeleteExpiredShoppingCartItems( )
        {
            DateTime expdate = DateTime.UtcNow.AddDays(-60);
            var sql = "delete from customer_carts WHERE date_created > @date_created; ";
            using (var connection = this.GetOpenConnection())
            {
                var result = await connection.ExecuteAsync(sql, new { date_created = expdate });
                return result;
            }
        }  
        public async Task<IEnumerable<CartItem>> GetCustomerCartItems(long customer_id)
        { 
            var sqlQuery = @"SELECT * FROM customer_carts  WHERE customer_id = @customer_id";
            logger.LogInformation(sqlQuery);
            using (var connection = this.GetOpenConnection())
            {
                var result = await connection.QueryAsync<CartItem>(sqlQuery);
                return result.ToList();
            }
        }

        public async Task<CustomerCart> GetCustomerCartAndCartItems(long customer_id, bool includeItems = false)
        {
            using (var connection = this.GetOpenConnection())
            {
                if (!includeItems)
                {
                    var sqlQuery = @"SELECT *
                                FROM customer_carts
                                WHERE customer_id = @customer_id";

                    var result = await connection.QueryAsync<CustomerCart>(sqlQuery, new { customer_id } );

                    return result.FirstOrDefault();
                }
                else
                {
                    var sqlQuery = @"
                    SELECT *
                    FROM customer_carts c
                    LEFT JOIN customer_cart_items ci on ci.cart_id = c.cart_id
                    WHERE c.customer_id = @customer_id";

                    var cartItems = new List<CartItem>();

                    var carts = await connection.QueryAsync<CustomerCart, CartItem, CustomerCart>(
                            sqlQuery, (cart, cartItem) =>
                            {
                                cartItems.Add(cartItem);
                                return cart;
                            },
                            param: new { customer_id } );

                    var result = carts.FirstOrDefault();
                    if (result != null)
                    {
                        result.cartItems = cartItems;
                    }

                    return result;
                }
            }
           
        }
    
    
    
    }
}
