using AppZeroAPI.Shared;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Linq;

namespace AppZeroAPI.Setup
{
    public class MyStripeOptions
    {
        public string Environment { get; set; } = "production";
        public string PublishableKey { get; set; } = "pk_test_51HiQuVAFZv6rpRFk3K1JeutsplKLBU7nFnti3wi6xZ6YW7sHUPJl433JQF4K9kSO0VsxX3edkIgrJrrbdzFPSGdt00a6LlFJ7W";// Environment.GetEnvironmentVariable("STRIPE_PUBLISHABLE_KEY");
        public string SecretKey { get; set; } = "sk_test_51HiQuVAFZv6rpRFkxiu0mnkJ35QnwdZtPHaXaWaqam4OlEIsLBDB5qphjD9lc38UWwjZwJlrdpd6BvYwLWCzogVu0075iwLofB";// Environment.GetEnvironmentVariable("STRIPE_SECRET_KEY");
        public string WebhookSecret { get; set; } = "";// Environment.GetEnvironmentVariable("STRIPE_WEBHOOK_SECRET");
        public string Price { get; set; } = "";//Environment.GetEnvironmentVariable("PRICE");
        public string Domain { get; set; } = "";// Environment.GetEnvironmentVariable("DOMAIN");


    }
    public static partial class SetUp
    {
        public static void ConfigureStripePay(this IServiceCollection services)
        {
            // c.IncludeXmlComments(string.Format(@"{0}\AppZeroAPI.AppZeroAPI.xml", System.AppDomain.CurrentDomain.BaseDirectory));
            services.Configure<MyStripeOptions>(options =>
            {
                options.PublishableKey = "pk_test_51HiQuVAFZv6rpRFk3K1JeutsplKLBU7nFnti3wi6xZ6YW7sHUPJl433JQF4K9kSO0VsxX3edkIgrJrrbdzFPSGdt00a6LlFJ7W";// Environment.GetEnvironmentVariable("STRIPE_PUBLISHABLE_KEY");
                options.SecretKey = "sk_test_51HiQuVAFZv6rpRFkxiu0mnkJ35QnwdZtPHaXaWaqam4OlEIsLBDB5qphjD9lc38UWwjZwJlrdpd6BvYwLWCzogVu0075iwLofB";// Environment.GetEnvironmentVariable("STRIPE_SECRET_KEY");
                options.WebhookSecret = Environment.GetEnvironmentVariable("STRIPE_WEBHOOK_SECRET");
                options.Price = Environment.GetEnvironmentVariable("PRICE");
                options.Domain = Environment.GetEnvironmentVariable("DOMAIN");
            });
        }
 
    }
}
