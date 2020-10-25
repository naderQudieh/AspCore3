using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

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
