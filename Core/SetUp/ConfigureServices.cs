using AppZeroAPI.Interfaces;
using AppZeroAPI.Repository;
using AppZeroAPI.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace AppZeroAPI.Setup
{
    public static partial class SetUp
    {
        public static void ConfigureServices(this IServiceCollection services)
        {
            // services.AddScoped<ILogRepository, LogRepository>();
            services.AddCoreServices();
            services.AddDataAccess();

        }


        public static void AddCoreServices(this IServiceCollection services)
        {
            services.AddSingleton<TokenService>();
            services.AddSingleton<BraintreeService>();  
            services.AddSingleton<EncryptorService>();
            services.AddTransient<IEmailService, EmailService>();
            services.AddTransient<IAuthService, AuthService>();
           
            
        }
        public static void AddDataAccess(this IServiceCollection services)
        {

            services.TryAddTransient<IUnitOfWork, UnitOfWork>();
            services.TryAddTransient<IProductRepository, ProductRepository>();
            services.TryAddTransient<IUserRepository, UserRepository>();
            services.TryAddTransient<IOrderRepository,  OrderRepository>();
            services.AddTransient<ILookupsRepository, LookupsRepository>();
            services.TryAddTransient<IPaymentRepository,  PaymentRepository>();
            services.TryAddTransient<ICustomerRepository,  CustomerRepository>();
        }
    }

}
