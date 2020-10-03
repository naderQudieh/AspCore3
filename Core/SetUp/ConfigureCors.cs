using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.DependencyInjection;
using AppZeroAPI.Entities;
using AppZeroAPI.Models;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace AppZeroAPI.Setup
{
    public static partial class SetUp
    {
        public static void ConfigureCors(this IServiceCollection services)
        {
 
            services.AddCors(opt =>
            {
                opt.AddPolicy("CorsPolicy", policy =>
                {
                    policy.AllowAnyHeader().AllowAnyMethod().WithOrigins().AllowCredentials();
                });
            });
        }
        
    }
 
}
