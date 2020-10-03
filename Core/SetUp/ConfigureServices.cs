using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using AppZeroAPI.Services;
using AppZeroAPI.Interfaces;
using AppZeroAPI.Repository;
using AppZeroAPI.Entities;
using AppZeroAPI.Models;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Data.SqlClient;
using System.Data;
using System.Data.Common;
using System.Configuration;
using Microsoft.Extensions.Configuration;

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
            services.AddSingleton<ITokenService, TokenService>();
            services.AddSingleton<EncryptorService>();
            services.AddTransient<IEmailService, EmailService>();
            services.AddTransient<IAuthService, AuthService>(); 
        }
        public static void AddDataAccess(this IServiceCollection services)
        {
            
            services.TryAddTransient<IUnitOfWork, UnitOfWork>();
            services.TryAddTransient<IProductRepository, ProductRepository>();
            services.TryAddTransient<IUserRepository, UserRepository>();
        ;
        } 
    }
    
}
