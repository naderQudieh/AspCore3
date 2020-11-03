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
    public class CartRepository : BaseRepository, ICartRepository
    {

        private readonly ILogger<CartRepository> logger;
        public CartRepository(IConfiguration configuration, ILogger<CartRepository> logger) : base(configuration)
        {
            this.logger = logger;
        }


        public async Task<IEnumerable<CustomerCart>> GetAllAsync()
        {

            //logger.LogInformation(sql);
            using (var connection = this.GetOpenConnection())
            {
                var result = await connection.GetAllAsync<CustomerCart>();
                return result.ToList();
            }
        }


        public async Task<int> AddAsync(CustomerCart entity)
        {
            using (var connection = this.GetOpenConnection())
            {
                entity.date_created = DateTime.UtcNow;
                entity.date_modified = DateTime.UtcNow;
                var result = await connection.InsertAsync(entity);
                return result;
            }
        }

        public async Task<int> AddDetailsAsync(CustomerCart entity)
        {
            using (var connection = this.GetOpenConnection())
            {
                entity.rec_id = Guid.NewGuid().ToString().ToLower().Replace("-", "");
                entity.date_created = DateTime.UtcNow;
                entity.date_modified = DateTime.UtcNow;
                var result = await connection.InsertAsync(entity);
                if (entity.cartItems != null && entity.cartItems.Count > 0)
                {
                    foreach (var item in entity.cartItems)
                    {
                        item.rec_id = Guid.NewGuid().ToString().ToLower().Replace("-", "");
                        item.cart_id = entity.rec_id;
                        var Id = (int)await connection.InsertAsync(item);
                    }
                }

                return result;
            }
        }

        public async Task<bool> DeleteByIdAsync(string rec_id)
        {
            using (var connection = this.GetOpenConnection())
            {
                var result = await connection.DeleteAsync(new CustomerCart() { rec_id = rec_id });
                return result;
            }
        }



        public async Task<CustomerCart> GetByIdAsync(string rec_id)
        {
            using (var connection = this.GetOpenConnection())
            {

                var result = await connection.GetAsync<CustomerCart>(new { rec_id = rec_id });
                return result;
            }
        }
        public async Task<bool> UpdateAsync(CustomerCart entity)
        {
            using (var connection = this.GetOpenConnection())
            {
                var isSuucess = await connection.UpdateAsync(entity);
                return isSuucess;
            }

        }


        public async Task<CustomerCart> GetCartDetails(string cart_id, bool includeItems = false)
        {
            using (var connection = this.GetOpenConnection())
            {
                if (!includeItems)
                {
                    var sqlQuery = @"SELECT *
                                FROM customer_carts
                                WHERE rec_id = @cart_id";

                    var result = await connection.QueryAsync<CustomerCart>(sqlQuery, new { cart_id });

                    return result.FirstOrDefault();
                }
                else
                {
                    var sqlQuery = @" SELECT * FROM customer_carts o
                    LEFT JOIN customer_cart_items oi on o.rec_id = oi.cart_id
                    WHERE c.rec_id = @cart_id";

                    var cartItems = new List<CartItem>();

                    var customerCart = await connection.QueryAsync<CustomerCart, CartItem, CustomerCart>(
                            sqlQuery, (cart, cartItem) =>
                            {
                                cartItems.Add(cartItem);
                                return cart;
                            },
                            param: new { cart_id }
                            ,
                            splitOn: "cart_id"

                            );

                    var result = customerCart.FirstOrDefault();
                    if (result != null)
                    {
                        result.cartItems = cartItems;
                    }

                    return result;
                }
            }

        }

        public async Task<CustomerCart> GetCustomerCartAndCartItems(string customer_id, bool includeItems = false)
        {
            using (var connection = this.GetOpenConnection())
            {
                if (!includeItems)
                {
                    var sqlQuery = @"SELECT *
                                FROM customer_carts
                                WHERE customer_id = @customer_id";

                    var result = await connection.QueryAsync<CustomerCart>(sqlQuery, new { customer_id });

                    return result.FirstOrDefault();
                }
                else
                {
                    var sqlQuery = @"
                    SELECT *
                    FROM customer_carts c
                    LEFT JOIN customer_cart_items ci on ci.cart_id = c.rec_id
                    WHERE c.customer_id = @customer_id";

                    var cartItems = new List<CartItem>();

                    var customerCart = await connection.QueryAsync<CustomerCart, CartItem, CustomerCart>(
                            sqlQuery, (cart, cartItem) =>
                            {
                                cartItems.Add(cartItem);
                                return cart;
                            },
                            param: new { customer_id }
                            ,
                            splitOn: "customer_id"

                            );

                    var result = customerCart.FirstOrDefault();
                    if (result != null)
                    {
                        result.cartItems = cartItems;
                    }

                    return result;
                }
            }

        }

        public async Task<IEnumerable<CartItem>> GetCustomerCartItems(string customer_id)
        {
            var sqlQuery = @"SELECT * FROM customer_carts  WHERE customer_id = @customer_id";
            logger.LogInformation(sqlQuery);
            using (var connection = this.GetOpenConnection())
            {
                var result = await connection.QueryAsync<CartItem>(sqlQuery);
                return result.ToList();
            }
        }

        public async Task<int> DeleteExpiredShoppingCartItems()
        {
            DateTime expdate = DateTime.UtcNow.AddDays(-60);
            var sql = "delete from customer_carts WHERE date_created > @date_created; ";
            using (var connection = this.GetOpenConnection())
            {
                var result = await connection.ExecuteAsync(sql, new { date_created = expdate });
                return result;
            }
        }

        public async Task<int> DeleteCartByCustomerId(string rec_id)
        {
            var sql = "delete from customer_cart  WHERE customer_id = @customer_id;   ";
            using (var connection = this.GetOpenConnection())
            {
                var result = await connection.ExecuteAsync(sql, new { customer_id = rec_id });
                return result;
            }
        }
        public async Task<int> DeleteCartById(string rec_id)
        {
            var sql = "delete from customer_carts WHERE cart_id = @cart_id; ";
            using (var connection = this.GetOpenConnection())
            {
                var result = await connection.ExecuteAsync(sql, new { rec_id = rec_id });
                return result;
            }
        }
        public async Task<int> DeleteCartItem(string cart_id, string cart_item_id)
        {
            var sql = "delete from customer_carts_items  WHERE cart_id = @cart_id and  cart_item_id=@cart_item_id ";
            using (var connection = this.GetOpenConnection())
            {
                var result = await connection.ExecuteAsync(sql, new { cart_id = cart_id, cart_item_id = cart_item_id });
                return result;
            }
        }
        /*

          public async Task<int> DeleteCustomerCartByIdAsync(string customer_id, string cart_id)
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
        public async Task<bool> UpdateCart(CustomerCart entity)
        {
            var sql = @"UPDATE customer_carts       SET product_id = @product_id,
                                 cart_id = @cart_id,    qty = @qty   WHERE Id = @Id";
            using (var connection = this.GetOpenConnection())
            {
                var isSuucess = await connection.UpdateAsync(entity);
                return isSuucess;
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
        */
    }
}
