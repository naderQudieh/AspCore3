using AppZeroAPI.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AppZeroAPI.Interfaces
{
    public interface ICartRepository : IGenericRepository<CustomerCart>
    {
        Task<int> AddDetailsAsync(CustomerCart entity);
        Task<CustomerCart> GetCartDetails(string cart_id, bool includeItems = false);
        Task<CustomerCart> GetCustomerCartAndCartItems(string customer_id, bool includeItems = false);
        Task<IEnumerable<CartItem>> GetCustomerCartItems(string customer_id);
        Task<int> DeleteExpiredShoppingCartItems();

        Task<int> DeleteCartByCustomerId(string customer_id);
        Task<int> DeleteCartItem(string cart_id, string cart_item_id);

    }
}
