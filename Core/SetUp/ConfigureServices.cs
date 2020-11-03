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
            services.AddSingleton<EncryptorService>();
            services.AddTransient<IEmailService, EmailService>();
            services.AddTransient<IAuthService, AuthService>();

           
            services.AddSingleton<PaymentBraintreeService>();
            services.AddSingleton<PaymentPaypalService>();
            services.AddSingleton<PaymentStripeService>();
           
           
        }
        public static void AddDataAccess(this IServiceCollection services)
        { 
         
            services.AddScoped<IUnitOfWork, UnitOfWork>(); 
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IOrderRepository,  OrderRepository>();
            services.AddScoped<ICartRepository, CartRepository>();
            services.AddScoped<ILookupsRepository, LookupsRepository>();
            services.AddScoped<IPaymentRepository,  PaymentRepository>();
            services.AddScoped<ICustomerRepository,  CustomerRepository>();

            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<ICustomerService, CustomerService>();
        }
    }

}
