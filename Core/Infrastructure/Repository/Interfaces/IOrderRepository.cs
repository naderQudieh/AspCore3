using AppZeroAPI.Entities;
using AppZeroAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AppZeroAPI.Interfaces
{
    public interface IOrderRepository : IGenericRepository<AppZeroAPI.Entities.CustomerOrder>
    {
        Task<CustomerOrder> GetOrderDetails(string order_id, bool includeItems = false);
        Task<IEnumerable<CustomerOrder>> GetOrdersForCustomer(string _customer_id);
        
    }
}
