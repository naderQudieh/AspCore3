using AppZeroAPI.Entities;
using AppZeroAPI.Interfaces;
using AppZeroAPI.Models;
using AppZeroAPI.Repository;
using AppZeroAPI.Shared;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AppZeroAPI.Shared.PayModel;
using System.Linq;
using Stripe;
using Stripe.Checkout;
using AppZeroAPI.Shared.Enums;

namespace AppZeroAPI.Services
{
   
    public class PaymentService : BaseRepository, IPaymentService
    {
        public string SiteBaseUrl = "";
        string ApiKey = "pk_test_51HiQuVAFZv6rpRFk3K1JeutsplKLBU7nFnti3wi6xZ6YW7sHUPJl433JQF4K9kSO0VsxX3edkIgrJrrbdzFPSGdt00a6LlFJ7W";
        string SecretKey = "sk_test_51HiQuVAFZv6rpRFkxiu0mnkJ35QnwdZtPHaXaWaqam4OlEIsLBDB5qphjD9lc38UWwjZwJlrdpd6BvYwLWCzogVu0075iwLofB";

        private readonly ILogger<PaymentService> logger; 
        private readonly ICustomerService customerService;
        public PaymentService(ICustomerService customerService, IConfiguration configuration, ILogger<PaymentService> logger) : base(configuration)
        {
            
            this.logger = logger;
            this.customerService = customerService;

        }


        public async Task<CustomerCart> CreateOrUpdatePaymentIntent(string customerId)
        {
            StripeConfiguration.ApiKey = SecretKey;// _config["StripeSettings:SecretKey"];

            var basket = await customerService.GetCartDetailsForCustomer(customerId);

            if (basket == null) return null;

            var shippingPrice = 0m;

            if (basket.deliveryMethodId.HasValue)
            {
                //var deliveryMethod = await unitOfWork..Repository<DeliveryMethod>()
                //    .GetByIdAsync((int)basket.DeliveryMethodId);
                // shippingPrice = deliveryMethod.Price;
            } 

            var service = new PaymentIntentService();

            PaymentIntent intent;

            if (string.IsNullOrEmpty(basket.paymentIntentId))
            {
                var options = new PaymentIntentCreateOptions
                {
                    Amount = (long)basket.cartItems.Sum(i => i.qty * (i.total_payable * 100)) + (long)shippingPrice * 100,
                    Currency = "usd",
                    PaymentMethodTypes = new List<string> { "card" }
                };
                intent = await service.CreateAsync(options);
                basket.paymentIntentId = intent.Id;
                basket.client_secret  = intent.ClientSecret;
            }
            else
            {
                var options = new PaymentIntentUpdateOptions
                {
                    Amount = (long)basket.cartItems.Sum(i => i.qty * (i.total_payable * 100)) + (long)shippingPrice * 100,
                };
                await service.UpdateAsync(basket.paymentIntentId, options);
            }

            await customerService.UpdateCart(basket);

            return basket;
        }

        public async Task<bool> updateOrderPaymentfailed(string  paymentId)
        { 
            return await customerService.UpdatePaymentStatus(paymentId, PaymentStatus.Failure.ToString()); 
        }

        public async Task<bool> updateOrderPaymentSucceeded(string paymentId)
        {
            return await customerService.UpdatePaymentStatus(paymentId, PaymentStatus.Paid.ToString());
        }
        
    }
}