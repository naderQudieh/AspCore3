using AppZeroAPI.Middleware;
using AppZeroAPI.Setup;
using AppZeroAPI.Shared;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;
using WebApi.Shared;

namespace AppZeroAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            ///services.AddDbContext<CartContext>(x => x.UseInMemoryDatabase("Server=DESKTOP-I7816G0;Database=ShopDb;Trusted_Connection=true;"));
           /// services.AddDbContext<ProductsContext>(x => x.UseInMemoryDatabase("Server=DESKTOP-I7816G0;Database=ShopDb;Trusted_Connection=true;"));

            services.AddHttpContextAccessor();
            services.AddSingleton<IObjectModelValidator, NullObjectModelValidator>();
            // in memory database used for simplicity, change to a real db for production applications
            //services.AddDbContext<DataContext>(x => x.UseInMemoryDatabase("TestDb")); 
            services.AddDbContext<DataContext>(opt =>
            {
                // opt => opt.UseInMemoryDatabase("TestDb")
                opt.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
            });

            services.ConfigureAutoMapper();
            services.ConfigureFluentMapper();
            services.ConfigureCors();
          
            services.AddControllers().ConfigureApiBehaviorOptions(options =>
            {
                options.SuppressConsumesConstraintForFormFileParameters = true;
                options.SuppressInferBindingSourcesForParameters = true;
                options.SuppressModelStateInvalidFilter = true;
            }) 
                .AddNewtonsoftJson(options =>
                     options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore)
                .AddJsonOptions(x => x.JsonSerializerOptions.IgnoreNullValues = true)
                
                .AddMvcOptions(options =>
                {
                    // NamingStrategy = new SnakeCaseNamingStrategy(),
                    options.InputFormatters.Insert(0, new JsonStringInputFormatter());
                });
            var appSettingsSection = Configuration.GetSection("JwtOptions");
            services.Configure<JwtOptions>(appSettingsSection); 
            services.ConfigureAuth(Configuration);
            // configure DI for application services
            //services.AddScoped<IUserService, UserService>();
            services.ConfigureServices();
            services.ConfigureSwagger();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, DataContext context)
        {
            app.UseMiddleware<ExceptionMiddleware>();
            // add hardcoded test user to db on startup
            // plain text password is used for simplicity, hashed passwords should be used in production applications
            //context.Users.Add(new User { FirstName = "Test", LastName = "User", Username = "test", Password = "test" });
            //context.Users.Add(new User { FirstName = "Test", LastName = "User", Username = "aaaa", Password = "aaaa" });
            context.SaveChanges();

            app.UseRouting();

            // global cors policy
            app.UseCors(x => x
                .SetIsOriginAllowed(origin => true)
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials());

            app.UseAuthentication();

            app.UseMiddleware<ExpiredTokenMiddleware>();
            app.UseAuthorization();
            app.UseConfiguredSwagger();
            app.UseEndpoints(x => x.MapControllers());
        }
    }
}
