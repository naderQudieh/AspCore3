using PayPalCheckoutSdk.Core;
using PayPalCheckoutSdk.Orders;
using AppZeroAPI.Shared.PayModel;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using PayPalHttp;
using System;

namespace AppZeroAPI.Services
{
    public class PaymentPaypalService
    {
        private static readonly string ClientId = "AXsvzo8YSrsScloTKO_Ufg4FXR1f4Le_DA9_j7Htli9qYQrXxTGbolQNueitJvY7ueTSFCR52zX92Nwm";
        private static readonly string ClientSecret = "EPmO4vP52YKdD6R0Y1rEq1JDhZPER4qup6T82s45Q2EXO7MZaJ7f5y8v_kW1pr-rbVvWjLxJWZAEaZBr";

        readonly PayPalHttpClient m_PayPalHttpClient;
        readonly ApplicationContext m_ApplicationContext;
        readonly decimal m_TaxRate;
        public static PayPalEnvironment environment()
        {

            return new SandboxEnvironment(
                 System.Environment.GetEnvironmentVariable("PAYPAL_CLIENT_ID") != null ?
                 System.Environment.GetEnvironmentVariable("PAYPAL_CLIENT_ID") : "AXsvzo8YSrsScloTKO_Ufg4FXR1f4Le_DA9_j7Htli9qYQrXxTGbolQNueitJvY7ueTSFCR52zX92Nwm",
                 System.Environment.GetEnvironmentVariable("PAYPAL_CLIENT_SECRET") != null ?
                 System.Environment.GetEnvironmentVariable("PAYPAL_CLIENT_SECRET") : "EPmO4vP52YKdD6R0Y1rEq1JDhZPER4qup6T82s45Q2EXO7MZaJ7f5y8v_kW1pr-rbVvWjLxJWZAEaZBr");
        }
        public PaymentPaypalService()
        {

            string SiteUrl = "http://www.appzero.com/api/payments/";
            var environment = new LiveEnvironment(ClientId, ClientSecret);
            m_PayPalHttpClient = new PayPalHttpClient(environment);
            m_ApplicationContext = new ApplicationContext
            {
                BrandName = "Naobiz",
                ReturnUrl = SiteUrl + "comporder",
                CancelUrl = SiteUrl + "canorder"
            };
            m_TaxRate = 5.5M;
        }

        public async Task<string> CreateOrder(AppZeroAPI.Entities.CustomerOrder order)
        {
            var request = new OrdersCreateRequest();
            request.Prefer("return=representation");
            request.RequestBody(CreateOrderRequest(order));
            var response = await m_PayPalHttpClient.Execute(request);
            var order2 = response.Result<PayPalCheckoutSdk.Orders.Order>();
            order.paypal_token    = order2.Id;
            var link = order2.Links.Single(link => link.Rel == "approve");
            return link.Href;
        }

        private OrderRequest CreateOrderRequest(AppZeroAPI.Entities.CustomerOrder order)
        {
            var request = new OrderRequest
            {
                ApplicationContext = m_ApplicationContext,
                CheckoutPaymentIntent = "CAPTURE"
            };
            var purchaseUnitRequest = new PurchaseUnitRequest
            {
                InvoiceId = order.rec_id.ToString(),
                AmountWithBreakdown = new AmountWithBreakdown
                {
                    Value = order.order_total.ToString("f2", CultureInfo.InvariantCulture),
                    CurrencyCode = "EUR",
                    AmountBreakdown = new AmountBreakdown
                    {
                        ItemTotal = new Money
                        {
                            Value = order.order_total.ToString("f2", CultureInfo.InvariantCulture),
                            CurrencyCode = "EUR"
                        }
                    }
                }
            };
            request.PurchaseUnits = new List<PurchaseUnitRequest>() { purchaseUnitRequest };
            return request;
        }

        public async Task<bool> CaptureOrder(string token)
        {
            var request = new OrdersCaptureRequest(token);
            request.Prefer("return=representation");
            request.RequestBody(new OrderActionRequest());
            var response = await m_PayPalHttpClient.Execute(request);
            var order2 = response.Result<PayPalCheckoutSdk.Orders.Order>();
            return order2.Status == "COMPLETED";
        }

        public PayPalHttpClient getClient()
        {
            // Creating a sandbox environment
            var environment = new SandboxEnvironment(ClientId, ClientSecret);

            // Creating a client for the environment
            var client = new PayPalHttpClient(environment);
            return client;
        }

        public async static Task<HttpResponse> CreateOrder2(bool debug = false)
        {
            var request = new OrdersCreateRequest();
            request.Prefer("return=representation");
            request.RequestBody(BuildRequestBody());
            PayPalHttpClient m_PayPalHttpClient = new PayPalHttpClient(environment());
            var response = await m_PayPalHttpClient.Execute(request);

            if (debug)
            {
                var result = response.Result<PayPalCheckoutSdk.Orders.Order>();
                Console.WriteLine("Status: {0}", result.Status);
                Console.WriteLine("Order Id: {0}", result.Id);
                Console.WriteLine("Intent: {0}", result.CheckoutPaymentIntent);
                Console.WriteLine("Links:");
                foreach (LinkDescription link in result.Links)
                {
                    Console.WriteLine("\t{0}: {1}\tCall Type: {2}", link.Rel, link.Href, link.Method);
                }
                AmountWithBreakdown amount = result.PurchaseUnits[0].AmountWithBreakdown;
                Console.WriteLine("Total Amount: {0} {1}", amount.CurrencyCode, amount.Value);
            }

            return response;
        }
        private static OrderRequest BuildRequestBody()
        {
            OrderRequest orderRequest = new OrderRequest()
            {
                CheckoutPaymentIntent = "CAPTURE",

                ApplicationContext = new ApplicationContext
                {
                    BrandName = "EXAMPLE INC",
                    LandingPage = "BILLING",
                    UserAction = "CONTINUE",
                    ShippingPreference = "SET_PROVIDED_ADDRESS"
                },
                PurchaseUnits = new List<PurchaseUnitRequest>
        {
          new PurchaseUnitRequest{
            ReferenceId =  "PUHF",
            Description = "Sporting Goods",
            CustomId = "CUST-HighFashions",
            SoftDescriptor = "HighFashions",
            AmountWithBreakdown = new AmountWithBreakdown
            {
              CurrencyCode = "BRL",
              Value = "230.00",
              AmountBreakdown = new AmountBreakdown
              {
                ItemTotal = new Money
                {
                  CurrencyCode = "BRL",
                  Value = "180.00"
                },
                Shipping = new Money
                {
                  CurrencyCode = "BRL",
                  Value = "30.00"
                },
                Handling = new Money
                {
                  CurrencyCode = "BRL",
                  Value = "10.00"
                },
                TaxTotal = new Money
                {
                  CurrencyCode = "BRL",
                  Value = "20.00"
                },
                ShippingDiscount = new Money
                {
                  CurrencyCode = "BRL",
                  Value = "10.00"
                }
              }
            },
            Items = new List<Item>
            {
              new Item
              {
                Name = "T-shirt",
                Description = "Green XL",
                Sku = "sku01",
                UnitAmount = new Money
                {
                  CurrencyCode = "BRL",
                  Value = "90.00"
                },
                Tax = new Money
                {
                  CurrencyCode = "BRL",
                  Value = "10.00"
                },
                Quantity = "1",
                Category = "PHYSICAL_GOODS"
              },
              new Item
              {
                Name = "Shoes",
                Description = "Running, Size 10.5",
                Sku = "sku02",
                UnitAmount = new Money
                {
                  CurrencyCode = "BRL",
                  Value = "45.00"
                },
                Tax = new Money
                {
                  CurrencyCode = "BRL",
                  Value = "5.00"
                },
                Quantity = "2",
                Category = "PHYSICAL_GOODS"
              }
            },
            ShippingDetail = new ShippingDetail
            {
              Name = new Name
              {
                FullName = "John Doe"
              },
              AddressPortable = new AddressPortable
              {
                AddressLine1 = "123 Townsend St",
                AddressLine2 = "Floor 6",
                AdminArea2 = "San Francisco",
                AdminArea1 = "CA",
                PostalCode = "94107",
                CountryCode = "US"
              }
            }
          }
        }
            };

            return orderRequest;
        }
    }
}
