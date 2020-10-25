using AppZeroAPI.Interfaces;
using AppZeroAPI.Repository;

namespace AppZeroAPI.Repository
{
    public class UnitOfWork : IUnitOfWork
    {

        public IProductRepository Products { get; }
        public IUserRepository Users { get; }
        public IOrderRepository Orders { get; }
        public IPaymentRepository Payments { get; }
        public ICustomerRepository Customers { get; }
        public ILookupsRepository Lookups { get; }
        public UnitOfWork(IProductRepository productRepository, IOrderRepository orderRepository, 
            IUserRepository userRepository, IPaymentRepository paymentRepository,
            ICustomerRepository customerRepository, ILookupsRepository lookupsRepository

            )
        {
            Payments = paymentRepository;
            Products = productRepository;
            Users = userRepository;
            Orders = orderRepository;
            Customers = customerRepository;
            Lookups = lookupsRepository;
        }

    }
}
