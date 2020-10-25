using AppZeroAPI.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AppZeroAPI.Interfaces
{
    public interface ICustomerRepository : IGenericRepository<Customer>
    { 
            Task<Cart> GetCustomerCartAndCartItems(long customer_id, bool includeItems = false);
            Task<IEnumerable<CartItem>> GetCustomerCartItems(long customer_id);
            Task<int> DeleteExpiredShoppingCartItems();
            Task<int> DeleteCartById(long cart_id);
            Task<int> DeleteCartByCustomerId(long customer_id);
            Task<int> DeleteCartItem(long cart_id, long cart_item_id);
            Task<bool> UpdateCartItem(CartItem entity);
            Task<long> AddCartItem(CartItem entity);
            Task<long> AddCart(Cart entity);
            Task<int> DeleteCustomerCartByIdAsync(long customer_id, long cart_id); 
    }

}
