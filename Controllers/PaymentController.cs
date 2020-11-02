﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Stripe;
using Stripe.Checkout;
using AppZeroAPI.Shared;
using System.IO;
using AppZeroAPI.Shared.Enums;
using AppZeroAPI.Models;
using AppZeroAPI.Interfaces;
using Microsoft.Extensions.Logging;
using AppZeroAPI.Services;
using AppZeroAPI.Shared.PayModel;

namespace AppZeroAPI.Controllers
{
 
    [ApiController]
    [Route("api/pay")]
    public class PaymentController : BaseController
    {

        public string SiteBaseUrl="";
        private static readonly string  publicKey = "AXsvzo8YSrsScloTKO_Ufg4FXR1f4Le_DA9_j7Htli9qYQrXxTGbolQNueitJvY7ueTSFCR52zX92Nwm";
        private static readonly string  secretKey = "EPmO4vP52YKdD6R0Y1rEq1JDhZPER4qup6T82s45Q2EXO7MZaJ7f5y8v_kW1pr-rbVvWjLxJWZAEaZBr";
        private readonly ILogger<PaymentController> logger;
        private readonly IUnitOfWork unitOfWork;
        private readonly PaymentStripeService stripeService;

        public PaymentController(PaymentStripeService stripeService, IUnitOfWork unitOfWork, ILogger<PaymentController> logger)
        {
            logger.LogInformation("called PaymentController");
            this.logger = logger;
            this.unitOfWork = unitOfWork;
            this.stripeService = stripeService;

            // StripeConfiguration.ApiKey = "sk_test_p9SPI9sPLAegWEXL6fZ5MIDm009gl2HLo5"; 
            StripeConfiguration.ApiKey = StripeOptions.SecretKey;
        }

        [HttpPost("RegisterCard")]
        public async Task<IActionResult> RegisterCard(AppZeroAPI.Entities.Customer customer)
        {
            var customerId = customer.customer_id;  
            var options = new SetupIntentCreateOptions
            {
                Customer = customerId.ToString(),
            };
            var service = new SetupIntentService();
            var intent = service.Create(options);
            var clientSecret = intent.ClientSecret;
            var response = await Task.FromResult(clientSecret);
            return Ok(response); 
        }

        [HttpGet("createSessionTest")]
        public async Task<IActionResult> StripeSessionTest()
        {
            var userUid = "2222";
            if (userUid == string.Empty)
                return Unauthorized();
            string sessionId="";

            try
            {
                var options = new PaymentIntentCreateOptions
                {
                    Amount = 1099,
                    Currency = "usd", 
                    Metadata = new Dictionary<string, string>()
                    {
                      {"integration_check", "accept_a_payment"},
                    }
                };

                var service = new PaymentIntentService();
                var paymentIntent = await service.CreateAsync(options).ConfigureAwait(false);
                sessionId = paymentIntent.ClientSecret;
            }
            catch (Exception ex)
            {
                
                return BadRequest();
            }

            return Ok(sessionId);
        }

         

        [HttpGet("teststripe")]
        public IActionResult TestStripe()
        {
            var options = new PaymentIntentCreateOptions
            {
                Amount = 1000,
                Currency = "usd",
                PaymentMethodTypes = new List<string> { "card" },
                ReceiptEmail = "jenny.rosen@example.com",
            };
            var service = new PaymentIntentService();
            var response = service.Create(options);

            return Ok(response);
        }


        [HttpPost]
        [Route("Buy")]
        public async Task<IActionResult> BuyAsync()
        {
           
            var options = new SessionCreateOptions
            {
                PaymentIntentData = new SessionPaymentIntentDataOptions
                {
                    CaptureMethod = "manual",
                    SetupFutureUsage = "off_session",
                },
                Customer = "cus_123",
                PaymentMethodTypes = new List<string> {
                    "card",
                },
                LineItems = new List<SessionLineItemOptions> {
                    new SessionLineItemOptions {
                        Name = "T-shirt",
                        Description = "Comfortable cotton t-shirt",
                        Amount = 500,
                        Currency = "usd",
                        Quantity = 1,
                    },
                } 
                ,
                SuccessUrl = "https://example.com/success",
                CancelUrl = "https://example.com/cancel",
            };

            var service = new SessionService();
            Session session = await service.CreateAsync(options);

            return Ok(session);
        }
        [HttpGet]
        [Route("GetSession")]
        public async Task<IActionResult> GetSessionAsync()
        {
           

            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> {
                    "card",
                },
                Mode = "setup",
                SuccessUrl = "http://localhost:3000/success?session_id={CHECKOUT_SESSION_ID}",
                CancelUrl = "http://localhost:3000/cancel",
            };

            var service = new SessionService();
            var session = await service.CreateAsync(options);

            return Ok(session);
        }

        [HttpGet("config")]
        public ActionResult<ConfigResponse> GetConfig()
        {
            var service = new PriceService();
            var price = service.Get(StripeOptions.Price);

            return new ConfigResponse
            {
                PublishableKey = StripeOptions.PublishableKey,
                UnitAmount = price.UnitAmount,
                Currency = price.Currency,
            };
        }

        [HttpGet("checkout-session")]
        public ActionResult<Session> GetCheckoutSession([FromQuery] string sessionId)
        {
            var service = new SessionService();
            var session = service.Get(sessionId); 
            return session;
        }

        [HttpPost("create-checkout-session")]
        public ActionResult<string> CreateCheckoutSession([FromBody]  int Quantity)
        {

            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string>
                {
                    "bacs_debit",
                },
                LineItems = new List<SessionLineItemOptions>
                {
                    new SessionLineItemOptions
                    {
                        Price = StripeOptions.Price,
                        Quantity = 1,
                    },
                },
                Mode = "payment",
                PaymentIntentData = new SessionPaymentIntentDataOptions
                {
                    SetupFutureUsage = "off_session",
                },

                SuccessUrl = StripeOptions.Domain + "/success.html?session_id={CHECKOUT_SESSION_ID}",
                CancelUrl = StripeOptions.Domain + "/canceled.html",
            };

            var service = new SessionService();
            Session session = service.Create(options);

            return session.Id;
        }

        [HttpPost("webhook")]
        public async Task<IActionResult> Webhook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            Event stripeEvent;
            try
            {
                stripeEvent = EventUtility.ConstructEvent(
                    json,
                    Request.Headers["Stripe-Signature"],
                    StripeOptions.WebhookSecret
                );
                Console.WriteLine($"Webhook notification with type: {stripeEvent.Type} found for {stripeEvent.Id}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Something failed {e}");
                return BadRequest();
            }

            switch (stripeEvent.Type)
            {
                case Events.CheckoutSessionCompleted:

                    System.Diagnostics.Debug.WriteLine("Checkout session completed");

                    break;
                case Events.CheckoutSessionAsyncPaymentSucceeded:

                    System.Diagnostics.Debug.WriteLine("Checkout session async payment succeeded");

                    break;
                case Events.CheckoutSessionAsyncPaymentFailed:

                    System.Diagnostics.Debug.WriteLine("Checkout session async payment failed");

                    break;
            }

            return Ok();
        }

        private async Task<ActionResult> PayWithStripeCheckout(AppZeroAPI.Entities.CustomerOrder order)
        {
            // Read Stripe API key from config
            StripeConfiguration.ApiKey = StripeOptions.SecretKey;

            // Add orderlines to Checkout session
            var lines = new List<SessionLineItemOptions>();
            foreach (var ol in order.orderItems)
            {
                
                var newline = new SessionLineItemOptions
                {
                    Name = ol.Product.name,
                    Description = ol.Product.description,
                    Amount = Convert.ToInt64(ol.total_payable ),
                    Currency = "usd",
                    Quantity = ol.qty
                };
                lines.Add(newline);
            }
            var options = new SessionCreateOptions
            {
                ClientReferenceId = order.order_id.ToString(),
                CustomerEmail = order.customer.email ,
                Locale = "nb",
                PaymentMethodTypes = new List<string> {
                    "card",
                },
                LineItems = lines,
                SuccessUrl = SiteBaseUrl + "/PaymentSuccess?session_id={CHECKOUT_SESSION_ID}",
                CancelUrl =  SiteBaseUrl + "/PaymentFailed",
            };

            var service = new SessionService();
            Session session = await service.CreateAsync(options);

            order.PaymentProviderSessionId = session.Id;
            //_context.Update(order);
            //await _context.SaveChangesAsync(); 
            return Ok(session);
        }
         
        private async Task<ActionResult> PayWithStripeElements(AppZeroAPI.Entities.CustomerOrder order)
        {
            // Read Stripe API key from config
            StripeConfiguration.ApiKey = StripeOptions.SecretKey;

            var paymentIntentCreateOptions = new PaymentIntentCreateOptions
            {
                Customer = StripeCustomer(order).Id,
                Amount = Convert.ToInt32(order.total_payable),
                Currency = "usd",
                PaymentMethodTypes = new List<string> { "card" },
                Description = "Bestilling fra Losvik kommune",
                ReceiptEmail = order.customer.email,
                StatementDescriptor = "Losvik kommune",
                Metadata = new Dictionary<String, String>()
                {
                    { "OrderId", order.order_id.ToString()}
                }
            };

            var service = new PaymentIntentService();
            var intent = await service.CreateAsync(paymentIntentCreateOptions);

            return Ok(intent);
        }


        [HttpPost]
        public async Task<ActionResult> Pay([FromBody] string order_id)
        {

            //var order = await _context.Orders
            //    .Where(p => p.Id == payOrder.OrderId)
            //    .Include(order => order.OrderLines)
            //    .FirstOrDefaultAsync();

            var order = new AppZeroAPI.Entities.CustomerOrder();
            if (order == null)
            {
                return BadRequest("No order found");
            }

            switch (order.PaymentProvider)
            {
                case PaymentProviderType.StripeCheckout:
                   
                    return await PayWithStripeCheckout(order);

                case PaymentProviderType.StripeElements:
                   
                    return await PayWithStripeElements(order);

                case PaymentProviderType.Vipps:

                    ///var initiate = await _vippsApiClient.InitiatePayment(order, _vippsSettings);
                    /// return Ok(initiate);
                    return Ok();
                default:
                    return BadRequest();
            }
        }
        [HttpPost("capture-order/{id}")]
        private Task<ActionResult> CapturePayment([FromRoute] int id)
        {

            //var order = await _context.Orders
            //    .Where(p => p.Id == id)
            //    .Include(order => order.OrderLines)
            //    .FirstOrDefaultAsync();

            //var capture = await _vippsApiClient.CapturePayment(order, _vippsSettings);
            throw new NotImplementedException();

        }
        private Task<ActionResult> PayWithVipps(AppZeroAPI.Entities.CustomerOrder order)
        {
            throw new NotImplementedException();

            // return BadRequest();
        }
        private Task<ActionResult> PayWithStripeBilling(AppZeroAPI.Entities.CustomerOrder order)
        {
            throw new NotImplementedException();
        }
        private Customer StripeCustomer(AppZeroAPI.Entities.CustomerOrder order)
        {
            var options = new CustomerCreateOptions
            {
                Name = order.customer.first_name + " " + order.customer.last_name,
                Email = order.customer.email,
                Phone = order.customer.contact_mobile,
                PreferredLocales = new List<string> { "nb", "en" }
            };

            var service = new CustomerService();
            var customer = service.Create(options);

            return customer;
        }

        [HttpPost("PurchaseItem")]
        public  IActionResult  PurchaseItem(string json)
        {
            AppZeroAPI.Entities.CustomerOrder purchaseOrder = JsonConvert.DeserializeObject<AppZeroAPI.Entities.CustomerOrder>(json);

            //var tokenVar = purchaseOrder.tokenVar;
            //Item[] items = purchaseOrder.Items;

            //Get the customer id
            var customerId = "11111";// await GetCustomer(tokenVar);

            var pmlOptions = new PaymentMethodListOptions
            {
                Customer = customerId,
                Type = "card",
            };

            //IF THERE ARENT ANY THAN THROW AN ERROR!!!

            var pmService = new PaymentMethodService();
            var paymentMethods = pmService.List(pmlOptions);

            var paymentIntents = new PaymentIntentService();
            var paymentIntent = paymentIntents.Create(new PaymentIntentCreateOptions
            {
                Customer = customerId,
                SetupFutureUsage = "off_session",
                Amount = 1000,
                Currency = "usd",
                PaymentMethod = paymentMethods.Data[0].Id,
                Description = "Name of items here"
            }); ; ;

            return Ok(new { client_secret = paymentIntent.ClientSecret });
        }

        [HttpPost("create-subscription")]
        public ActionResult<Subscription> CreateSubscription([FromBody] CreateSubscriptionRequest req)
        {
            // Attach payment method
            var options = new PaymentMethodAttachOptions
            {
                Customer = req.Customer,
            };
            var service = new PaymentMethodService();
            var paymentMethod = service.Attach(req.PaymentMethod, options);

            // Update customer's default invoice payment method
            var customerOptions = new CustomerUpdateOptions
            {
                InvoiceSettings = new CustomerInvoiceSettingsOptions
                {
                    DefaultPaymentMethod = paymentMethod.Id,
                },
            };
            var customerService = new CustomerService();
            customerService.Update(req.Customer, customerOptions);

            // Create subscription
            var subscriptionOptions = new SubscriptionCreateOptions
            {
                Customer = req.Customer,
                Items = new List<SubscriptionItemOptions>
                {
                    new SubscriptionItemOptions
                    {
                        Price = Environment.GetEnvironmentVariable(req.Price),
                    },
                },
            };
            subscriptionOptions.AddExpand("latest_invoice.payment_intent");
            var subscriptionService = new SubscriptionService();
            try
            {
                Subscription subscription = subscriptionService.Create(subscriptionOptions);
                return subscription;
            }
            catch (StripeException e)
            {
                Console.WriteLine($"Failed to create subscription.{e}");
                return BadRequest();
            }
        }

        [HttpPost("cancel-subscription")]
        public ActionResult<Subscription> CancelSubscription([FromBody] string subscription )
        {
            var service = new SubscriptionService();
            return service.Cancel(subscription, null);
            
        }

        [HttpPost("update-subscription")]
        public ActionResult<Subscription> UpdateSubscription([FromBody] string Subscription, string NewPrice)
        {
            var service = new SubscriptionService();
            var subscription = service.Get(Subscription);

            var options = new SubscriptionUpdateOptions
            {
                CancelAtPeriodEnd = false,
                Items = new List<SubscriptionItemOptions>
                {
                    new SubscriptionItemOptions
                    {
                        Id = subscription.Items.Data[0].Id,
                        Price = Environment.GetEnvironmentVariable(NewPrice),
                    }
                }
            };
            var updatedSubscription = service.Update(Subscription, options);
            return updatedSubscription;
        }

        [HttpPost("retrieve-customer-payment-method")]
        public IActionResult RetrieveCustomerPaymentMethod([FromBody] string paymentMethod  )
        {
            var service = new PaymentMethodService();
            var res = service.Get(paymentMethod);
            return Ok(res);
        }
    }
}
 
   
 