using Microsoft.Extensions.DependencyInjection;

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
