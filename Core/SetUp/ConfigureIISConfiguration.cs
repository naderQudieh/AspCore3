using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppZeroAPI.Setup
{
    public static partial class SetUp
    {
        public static void ConfigureIISConfiguration(this IServiceCollection services)
        {
            services.Configure<IISOptions>(iis =>
            { 
                iis.AutomaticAuthentication = false;
            });

            
        }
        
     
    }
}
