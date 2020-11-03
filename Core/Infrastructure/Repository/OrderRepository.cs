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
                    var sqlQuery = @"SELECT *
                                FROM customer_orders
                                WHERE rec_id = @order_id";

                    var result = await connection.QueryAsync<CustomerOrder>(sqlQuery, new { order_id });

                    return result.FirstOrDefault();
                }
                else
                {
                    var sqlQuery = @" SELECT * FROM customer_orders o
                    LEFT JOIN customer_order_items oi on o.rec_id = oi.order_id
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
           // entity.ModifiedOn = DateTime.UtcNow;
            var sql = "UPDATE Orders SET Name = @Name, Description = @Description, Barcode = @Barcode, Rate = @Rate, ModifiedOn = @ModifiedOn  WHERE Id = @Id";
            using (var connection = this.GetOpenConnection())
            {
                var rows = await connection.ExecuteAsync(sql, entity);
                return rows != 0;
            }
        }

        public async Task<CustomerOrder> CreateOrder(Guid userUid, string ipAddress)
        {
            const string sql = @"
DECLARE @OrderId TABLE ([Id] INT);
DECLARE @UserId INT = (SELECT Id FROM SiteUser WHERE [Uid] = @UserUid AND SiteId = @SiteId);
DECLARE @Email NVARCHAR(200) = (SELECT EmailAddress FROM SiteUser WHERE [Uid] = @UserUid AND SiteId = @SiteId);
INSERT INTO Orders
OUTPUT INSERTED.[Id] INTO @OrderId
SELECT NEWID(), @UserId, NULL, NULL, @Email, 1, GETDATE(), NULL, NULL, NULL, NULL, 0.00, NULL, 8.99, NULL, 0.00, @IPAddress, GETDATE(), GETDATE();
SELECT Id, [Uid], UserId, ShippingId, BillingId, Email, OrderStateId, OrderDate, ProcessedDate, 
    TokenId, CardAuth, Subtotal, Tax, ShippingCost, Discount, Total, IPAddress, CreatedDate, LastModifiedDate
FROM Orders WHERE Id = (SELECT Id FROM @OrderId);
";
            using (var connection = this.GetOpenConnection())
            {

                var result = await connection.QuerySingleOrDefaultAsync<CustomerOrder>(sql, new { Id = 1 });
                return result;
            }
        }

        public async Task<List<CustomerOrderItem>> CreateOrderItemsFromCart(Guid userUid, int orderId)
        {
            const string sql = @"
DECLARE @OrderItemIds TABLE ([Id] INT);
--insert order items from cart
INSERT INTO OrderItem
OUTPUT INSERTED.[Id] INTO @OrderItemIds
SELECT NEWID(), @OrderId, ci.Id, ci.Quantity, ci.Price, ci.Price * .11, 8.99, NULL, '', 0, GETDATE()
FROM CartItem ci 
LEFT JOIN Cart c ON c.Id = ci.CartId 
WHERE 1=1 
AND c.UserUid = @UserUid 
AND ci.Active = 1 
AND ci.Quantity > 0
AND ci.Price >= 0;
--update orders totals from order items
;WITH SumOrderItems AS (
	SELECT OrderId, SUM(ISNULL(Price, 0)) AS Subtotal, SUM(ISNULL(Tax, 0)) AS Tax, SUM(ISNULL(Discount, 0)) AS Discount FROM OrderItem WHERE OrderId = @OrderId GROUP BY OrderId 
)
UPDATE
    Orders
SET
    Subtotal = soi.Subtotal, Tax = soi.Tax, Discount = soi.Discount, Total = soi.Subtotal + soi.Tax + o.ShippingCost - soi.Discount
FROM
    Orders o
INNER JOIN
    SumOrderItems soi
ON 
    o.Id = soi.OrderId;
--return order items
SELECT oi.Id, oi.[Uid], oi.OrderId, oi.CartItemId, oi.Quantity, oi.Price, oi.Tax, oi.ShippingCost, oi.Discount, oi.Tracking, oi.IsBackorder, oi.CreatedDate,
    si.Name, si.Description, si.SmallImg
FROM OrderItem oi
JOIN CartItem ci ON ci.Id = oi.CartItemId
JOIN StoreItem si ON si.Id = ci.ItemId
WHERE OrderId = @OrderId;
";
            using (var connection = this.GetOpenConnection())
            {

                var result = await connection.QueryAsync<CustomerOrderItem>(sql, new { Id = 1 });
                return result.ToList();
            }
           
        }
         
       
    }
}
