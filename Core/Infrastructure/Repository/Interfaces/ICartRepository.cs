using AppZeroAPI.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AppZeroAPI.Interfaces
{
    public interface ICartRepository : IGenericRepository<CustomerCart>
    {
        Task<int> AddDetailsAsync(CustomerCart entity);
        Task<CustomerCart> GetCartDetails(string cart_id, bool includeItems = false);
        Task<CustomerCart> GetCustomerCartDetails(string customer_id, bool includeItems = false);
        Task<IEnumerable<CartItem>> GetCustomerCartItems(string customer_id);
        Task<int> DeleteExpiredShoppingCartItems();
        Task<bool> DeleteCustomerCart(string rec_id);
        Task<int> DeleteCartByCustomerId(string customer_id);
        Task<int> DeleteCartItems(string cart_id);
        Task<int> DeleteCartItem(string cart_id, string cart_item_id);

    }
}
