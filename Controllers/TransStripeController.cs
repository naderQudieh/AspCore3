using AppZeroAPI.Interfaces;
using AppZeroAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Newtonsoft.Json;
using Stripe;
using Stripe.Checkout;
using AppZeroAPI.Shared.Enums;
using AppZeroAPI.Shared.PayModel;
using AppZeroAPI.Entities;
using System.Text.Json;
using PaymentMethod = Stripe.PaymentMethod;
using Order = Stripe.Order ;
using CustomerService = Stripe.CustomerService;
using AppZeroAPI.Shared;

namespace AppZeroAPI.Controllers
{
 
    [ApiController]
    [Route("api/paystripe")]
    public class TransStripeController : BaseController
    {

        public string SiteBaseUrl="";
        private static readonly string publicKey = "pk_test_51HiQuVAFZv6rpRFk3K1JeutsplKLBU7nFnti3wi6xZ6YW7sHUPJl433JQF4K9kSO0VsxX3edkIgrJrrbdzFPSGdt00a6LlFJ7W";
        private static readonly string secretKey = "sk_test_51HiQuVAFZv6rpRFkxiu0mnkJ35QnwdZtPHaXaWaqam4OlEIsLBDB5qphjD9lc38UWwjZwJlrdpd6BvYwLWCzogVu0075iwLofB";
        private readonly ILogger<TransStripeController> logger;
        private readonly IUnitOfWork unitOfWork;
        private readonly PaymentStripeService stripeService;

        public TransStripeController(PaymentStripeService stripeService, IUnitOfWork unitOfWork, ILogger<TransStripeController> logger)
        {
            StripeConfiguration.ApiKey = secretKey;// StripeOptions.SecretKey;
            logger.LogInformation("called PaymentController");
            this.logger = logger;
            this.unitOfWork = unitOfWork;
            //this.stripeService = stripeService;
        }

       
        [HttpPost("CreateCardPaymentMethod")]
        public async Task<IActionResult> CreateCardPaymentMethod()
        {
            PayModel paymodel = getPayModel();
            var options = new PaymentMethodCreateOptions
            {
                Customer = "Nahed Kadih",
                Type = "card",
                Card = new PaymentMethodCardOptions
                {
                    Number = paymodel.CardNumder,
                    ExpMonth = paymodel.ExpMonth,
                    ExpYear = paymodel.ExpYear,
                    Cvc = paymodel.CVC,
                },
                BillingDetails = new PaymentMethodBillingDetailsOptions
                {
                    Name = "Nahed Kadih",
                    Address = new Stripe.AddressOptions
                    {
                        PostalCode = paymodel.AddressZip,
                        City = paymodel.AddressCity
                    },
                    Email = "markchristopher.cacal@gmail.com",
                    Phone = "09067701852"
                },
            };
            var paymentMethodService = new PaymentMethodService();
            PaymentMethod paaymentMethoden = paymentMethodService.Create(options);

            // `source` is obtained with Stripe.js; see https://stripe.com/docs/payments/accept-a-payment-charges#web-create-token
            //var chargeCreateOptions = new ChargeCreateOptions
            //{
            //    Amount = 2000,
            //    Currency = "usd",
            //    Source = "tok_visa",
            //    Description = "Charge for jenny.rosen@example.com",

            //};
            //var chargeService = new ChargeService();
            //var iRes = chargeService.Create(chargeCreateOptions);

            var response = await Task.FromResult(paaymentMethoden);
            return Ok(response);
        }

        [HttpPost("getOrCreateCustomer")]
        public async Task<IActionResult> getOrCreateCustomer()
        {
            PayModel paymodel = getPayModel();
            var service = new CustomerService();
            var listOptions = new CustomerListOptions
            {
                Limit = 1
            };
            listOptions.AddExtraParam("email", paymodel.Email);
            var customer = (await service.ListAsync(listOptions)).Data.FirstOrDefault();
            if (customer != null)
            {
                return Ok(customer);
            }

            var options = new CustomerCreateOptions
            {
                Email = paymodel.Email,
                Phone = paymodel.Phone,
                Name = paymodel.Name,
                Address = new AddressOptions()
                {
                    Line1 = paymodel.AddressLine1,
                    Line2 = paymodel.AddressLine2,
                    State = paymodel.AddressState,
                    City = paymodel.AddressCity,
                    Country = paymodel.AddressCountry,
                    PostalCode = paymodel.AddressZip
                },
                Metadata = new Dictionary<string, string>() {
                    {"TrainingYears", "user.TrainingYears" },
                    {"GroupName", "user.GroupName" },
                    {"Level", "user.Level" }
                },
            };
            
            var result = await service.CreateAsync(options); 
            var response = await Task.FromResult(result);
            return Ok(response);
        }
         

        [HttpPost("CreateCustomerAsync")]
        public async Task<IActionResult> CreateCustomerAsync()
        {
            PayModel paymodel = getPayModel();
            var options = new CustomerCreateOptions
            {
                Source = this.Token().Id,
                Name = paymodel.Name,
                Email = paymodel.Email,
                Phone = paymodel.Phone
            };

            var service = new CustomerService();
            Stripe.Customer customer = await service.CreateAsync(options);
            var response = await Task.FromResult(customer);
            return Ok(response);
        }

        [HttpPost("stripecustomer")]
        public async Task<IActionResult> CreateCustomer()
        {
            PayModel paymodel = getPayModel();
            var client = CreateStripeClient(paymodel.Email, paymodel.Name, paymodel.CardNumder, paymodel.ExpMonth, paymodel.ExpYear, paymodel.CVC);
            var response = await Task.FromResult(client);
            return Ok(response);
        }
        [Route("secret")]
        [HttpGet]
        public ActionResult Secret()
        {
            var options = new PaymentIntentCreateOptions
            {
                Amount = 1099,
                Currency = "USD",
                Metadata = new Dictionary<string, string>
            {
              { "integration_check", "accept_a_payment" },
            },
            };

            var service = new PaymentIntentService();
            var paymentIntent = service.Create(options);
            return Content(paymentIntent.ClientSecret);
        }


        [HttpGet]
        [Route("CreatePaymentIntent")] 
        public async Task<IActionResult> CreatePaymentIntent(long amount, string currency, string customerid)
        {
            var service = new PaymentIntentService();
            var options = new PaymentIntentCreateOptions
            {
                Amount = amount,
                Currency = currency,
                PaymentMethodTypes = new List<string> { "card" },
                Customer = customerid,
                Metadata = new Dictionary<string, string>
                {
                  { "integration_check", "accept_a_payment" },
                },
            };
            PaymentIntent PaymentIntent = service.Create(options);
            var response = await Task.FromResult(PaymentIntent);
            return Ok(response);
        }

        [Route("charge")]
        [HttpPost]
        public async Task<IActionResult> Charge(string stripeEmail, string stripeToken)
        {
           try
            {
                var options = new ChargeCreateOptions
                {

                    Amount = 123,
                    Currency = "usd",
                    Description = "ProductDescription",
                    //Source = this.Token().Id, //Token = tok_visa //PAYMENT METHOD  pm_card_visa
                    Source = new CardCreateNestedOptions
                    {
                        Name = "Nahed Kadih",
                        AddressLine1 = "8360 Greensboro Dr",
                        AddressLine2 = "626",
                        AddressCity = "McLean",
                        AddressState = "VA",
                        AddressZip = "22102", 
                        Number = "4242424242424242",
                        ExpMonth = 1,
                        ExpYear = 2021,
                        Cvc = "234"
                    },
                    StatementDescriptor = "WeSoft Charges Conf No- ",
                    Metadata = new Dictionary<string, string>
                      {
                        { "OrderId", "6735" },
                      },
                };

                var service = new ChargeService();
                var charge = service.CreateAsync(options); 
                var response = await Task.FromResult(charge); 
                return Ok(charge);
            }
            catch (Exception ex)
            {

                return BadRequest();
            }
           
            
        }
       
        [HttpPost("RegisterCard")]
        public async Task<IActionResult> RegisterCard(AppZeroAPI.Entities.Customer customer)
        {
            var customerId = customer.rec_id ;  
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
                    ReceiptEmail = "nkadih@yahoo.com",
                    Metadata = new Dictionary<string, string>()
                    {
                      {"integration_check", "accept_a_payment"},
                      { "OrderId", "6735" },
                    }
                },
                Customer = "cus_IJVSPOCEsKPVJW",
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

        private async Task<ActionResult> PayWithStripeCheckout([FromBody] CustomerOrder order)
        {
            
            StripeConfiguration.ApiKey = StripeOptions.SecretKey; 
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
                ClientReferenceId = order.rec_id.ToString(),
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
         
        private async Task<ActionResult> PayWithStripeElements([FromBody]  CustomerOrder order)
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
                    { "OrderId", order.rec_id.ToString()}
                }
            };

            var service = new PaymentIntentService();
            var intent = await service.CreateAsync(paymentIntentCreateOptions);

            return Ok(intent);
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
        private Task<ActionResult> PayWithVipps([FromBody]  CustomerOrder order)
        {
            throw new NotImplementedException();

            // return BadRequest();
        }
        private Task<ActionResult> PayWithStripeBilling(CustomerOrder order)
        {
            throw new NotImplementedException();
        }
        private Stripe.Customer StripeCustomer([FromBody]  CustomerOrder order)
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
        public  IActionResult  PurchaseItem([FromBody] CustomerOrder  purchaseOrder)
        {
        
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
        public ActionResult<Subscription> CreateSubscription([FromBody]  CreateSubscriptionRequest req)
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
        public ActionResult<Subscription> CancelSubscription([FromBody]  string _subscription   )
        {
            var service = new SubscriptionService();
            var subscription = service.Cancel(_subscription, null);
            return subscription;
        }

        [HttpPost("update-subscription")]
        public ActionResult<Subscription> UpdateSubscription([FromBody] string _subscription, string NewPrice)
        {
            var service = new SubscriptionService();
            var subscription = service.Get(_subscription);

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
            var updatedSubscription = service.Update(_subscription, options);
            return updatedSubscription;
        }

        [HttpPost("retrieve-customer-payment-method")]
        public ActionResult<Stripe.PaymentMethod> RetrieveCustomerPaymentMethod([FromBody] string paymentMethod)
        {
            var service = new PaymentMethodService();
            return service.Get(paymentMethod); 
        }


        private Token Token()
        {
            var options = new TokenCreateOptions
            {
                Card = new TokenCardOptions
                {
                    Name = "Nahed Kadih",
                    Number = "4242424242424242",
                    ExpMonth = 1,
                    ExpYear = 2021, 
                    Cvc = "234"
                }
            };

            var service = new Stripe.TokenService();

            return service.Create(options);
        }

        private async Task<string> CreateStripeClient(string email, string name, string cardNumber, int month, int year, string cvv)
        {
            var optionstoken = new TokenCreateOptions
            {
                Card = new TokenCardOptions
                {
                    Number = cardNumber,
                    ExpMonth = month,
                    ExpYear = year,
                    Cvc = cvv
                }
            };

            var servicetoken = new Stripe.TokenService();
            Token stripetoken = await servicetoken.CreateAsync(optionstoken);

            var customer = new CustomerCreateOptions
            {
                Email = email,
                Name = name,
                Source = stripetoken.Id,
            };

            Console.WriteLine(" stripetoken attributes :" + stripetoken);

            var services = new CustomerService();
            var created = services.Create(customer);


            var option = new PaymentMethodCreateOptions
            {
                Type = "card",
                Card = new PaymentMethodCardOptions
                {
                    Number = cardNumber,
                    ExpMonth = month,
                    ExpYear = year,
                    Cvc = cvv,
                },
            };

            var service = new PaymentMethodService();
            var result = service.Create(option);

            Console.WriteLine(" PaymentMethodService attributes :" + result);

            var options = new PaymentMethodAttachOptions
            {
                Customer = created.Id,
            };
            var method = new PaymentMethodService();
            method.Attach(
              result.Id,
              options
            );

            if (created.Id == null)
            {
                return "Failed";
            }
            else
            {
                return created.Id;
            }
        }
        private PayModel getPayModel()
        {
            var paymodel = new PayModel();
            paymodel.Amount = 200;
            paymodel.ExpMonth = 2;
            paymodel.ExpYear = 2022;
            paymodel.Name = "Nahed Kadih";
            paymodel.CardNumder = "4242424242424242";
            paymodel.CVC = "234";
            paymodel.OrderName = "OrderName";
            paymodel.OrderDescription = "OrderDescription";
            paymodel.AddressLine1 = "8360 Greensboro Dr";
            paymodel.AddressLine2 = "626";
            paymodel.AddressCity = "McLean";
            paymodel.AddressState = "VA";
            paymodel.AddressZip = "22102";
            paymodel.AddressCountry  = "USA";
            paymodel.Email = "customer1@gmail.com";
            return paymodel;
        }
    }

    
}
 
   
 