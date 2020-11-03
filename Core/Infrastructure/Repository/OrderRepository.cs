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
using AutoMapper;
using AppZeroAPI.Shared.Enums;

namespace AppZeroAPI.Repository
{
    public class OrderRepository : BaseRepository, IOrderRepository
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork unitOfWork;
        private readonly ILogger<OrderRepository> logger;
        public OrderRepository(IUnitOfWork unitOfWork, IMapper mapper,IConfiguration configuration, ILogger<OrderRepository> logger) : base(configuration)
        {
            _mapper = mapper;
            this.unitOfWork = unitOfWork;
            this.logger = logger;
        }


        public async Task<IEnumerable<CustomerOrder>> GetAllAsync()
        { 
            //logger.LogInformation(sql);
            using (var connection = this.GetOpenConnection())
            {
                var result = await connection.GetAllAsync<CustomerOrder>();
                return result.ToList();
            }
        }

        public async Task<IEnumerable<CustomerOrder>> GetOrdersForCustomer(string _customer_id)
        { 
            //logger.LogInformation(sql);
            using (var connection = this.GetOpenConnection())
            {
                var result = await connection.QueryAsync<CustomerOrder>(_customer_id );
                return result.ToList();
            }
        }

        public async Task<CustomerOrder> GetOrderDetails(string order_id, bool includeItems = false)
        {
            using (var connection = this.GetOpenConnection())
            {
                if (!includeItems)
                {
                    var sqlQuery = @"SELECT *   FROM customer_orders  WHERE rec_id = @order_id"; 
                    var result = await connection.QueryAsync<CustomerOrder>(sqlQuery, new { order_id }); 
                    return result.FirstOrDefault();
                }
                else
                {
                    var sqlQuery = @" SELECT * FROM customer_orders o  LEFT JOIN customer_order_items oi on o.rec_id = oi.order_id
                    WHERE c.rec_id = @rec_id";

                    var orderItems = new List<CustomerOrderItem>();

                    var orders = await connection.QueryAsync<CustomerOrder, CustomerOrderItem, CustomerOrder>(
                            sqlQuery, (order, orderItem) =>
                            {
                                orderItems.Add(orderItem);
                                return order;
                            },
                            param: new { order_id });

                    var result = orders.FirstOrDefault();
                    if (result != null)
                    {
                        result.orderItems = orderItems;
                    }

                    return result;
                }
            }

        }
      

        public async Task<int> AddAsync(CustomerOrder entity)
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
       


        public async Task<bool> DeleteByIdAsync(string rec_id)
        {
            using (var connection = this.GetOpenConnection())
            {
                var result = await connection.DeleteAsync(new CustomerOrder() { rec_id = rec_id });
                return result;
            }
        }
        


        public async Task<CustomerOrder> GetByIdAsync(string rec_id  )
        {
            using (var connection = this.GetOpenConnection())
            {

                var result = await connection.GetAsync<CustomerOrder>(new { id = rec_id });
                return result;
            }
        }


        public async Task<bool> UpdateAsync(CustomerOrder entity)
        {
            using (var connection = this.GetOpenConnection())
            {
                var isSuucess = await connection.UpdateAsync(entity);
                return isSuucess;
            }

        }

        public async Task<bool> UpdateOrderPaymentStatus(string paymentIntentId, OrderPaymentStatus order_pymnt_status)
        {
            var sql = @"UPDATE customer_orders SET  order_pymnt_status = @order_pymnt_status  WHERE paymentIntentId = @paymentIntentId";
            using (var connection = this.GetOpenConnection())
            {
                int rows = await connection.ExecuteAsync(sql, new { order_pymnt_status = order_pymnt_status.ToString(), paymentIntentId = paymentIntentId });
                return rows != 0;
            }
        }
        public async Task<bool> UpdateOrderStatus(string order_id, OrderStatus order_status)
        {
            
            var sql = @"UPDATE customer_orders SET  order_status = @order_status  WHERE rec_id = @order_id";
            using (var connection = this.GetOpenConnection())
            {
                int rows = await connection.ExecuteAsync(sql, new { order_id = order_id, order_status = order_status.ToString() });
                return rows != 0;
            }
        }
        public async Task<CustomerOrder> CreateOrderItemsFromCart(string cart_id)
        {
            CustomerOrder order = new CustomerOrder();
            try
            {
                CustomerCart cart = await unitOfWork.Carts.GetCartDetails(cart_id, true);
                using (var connection = this.GetOpenConnection())
                {
                    
                    order.customer_id = cart.customer_id;
                    order.order_total = cart.cart_total;
                    order.total_payable = cart.total_payable;
                    order.discount_amount = cart.cart_discount;
                    var result = await connection.InsertAsync(order);
                    if (cart.cartItems != null && cart.cartItems.Count > 0)
                    {
                        foreach (var item in cart.cartItems)
                        {
                            CustomerOrderItem orderitem = new CustomerOrderItem(); 
                            orderitem.rec_id = Guid.NewGuid().ToString().ToLower().Replace("-", "");
                            orderitem.order_id = order.rec_id;
                            orderitem.qty = item.qty;
                            orderitem.price = item.price;
                            orderitem.total_payable = item.total_payable;
                            orderitem.product_id = item.product_id;
                            var Id = (int)await connection.InsertAsync(orderitem);
                            order.orderItems.Add(orderitem);
                        }
                    }

                }
            }
            catch (Exception ex)
            {

                return null;
            }
            
            return order;
        }
         
       
    }
}
