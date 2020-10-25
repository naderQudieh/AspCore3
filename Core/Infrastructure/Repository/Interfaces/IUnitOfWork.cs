using AppZeroAPI.Repository;

namespace AppZeroAPI.Interfaces
{
    public interface IUnitOfWork
    {
        IProductRepository Products { get; } 
        IOrderRepository Orders { get; }
        IUserRepository Users { get; }
        ICustomerRepository Customers { get; } 
        ILookupsRepository Lookups { get; }
        IPaymentRepository Payments { get; }
    }

}
