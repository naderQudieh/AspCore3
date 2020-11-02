using AppZeroAPI.Entities;
using AppZeroAPI.Interfaces;
using AppZeroAPI.Models;
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
using PayPalCheckoutSdk.Core;
using PayPalCheckoutSdk.Orders;
using PayPalHttp;

namespace AppZeroAPI.Controllers
{
    //[AuthorizeUser]
    [ApiController]
    [Route("api/paypal")]
    public class TransPayPalController : BaseController
    {
        private readonly ILogger<TransPayPalController> logger;
        private readonly IUnitOfWork unitOfWork;
        private readonly PaymentBraintreeService braintreeService;

        public TransPayPalController(PaymentBraintreeService braintreeService, IUnitOfWork unitOfWork, ILogger<TransPayPalController> logger)
        {
            logger.LogInformation("called PaymentController");
            this.logger = logger;
            this.unitOfWork = unitOfWork;
            this.braintreeService = braintreeService;
        }

        [HttpGet()]
        public IActionResult Get()
        {
            return Ok("PaymentController");
        }

        [HttpGet("pmt/comporder")]
        public IActionResult comporder()
        {
            return Ok("PaymentController");
        }


        [HttpGet("pmt/canorder")]
        public IActionResult postTransaction()
        {
            return Ok("PaymentController");
        }

        [HttpGet("pmt/return")]
        public IActionResult postTransaction([FromQuery] long customer_id)
        {
            return Ok("PaymentController");
        }
        //[HttpGet]
        //public IEnumerable<string> GetClientToken()
        //{
        //    logger.LogDebug("Getting client_token from braintree");
        //    var generatedToken = braintreeService.GetClientToken("234");
        //    return new string[] { "client_token", generatedToken };
        //}

        [HttpGet("pmt/ppmt")]
        public async Task<IActionResult> ProcessPayment([FromBody] long customer_id, string payment_id)
        {
            var data = await unitOfWork.Payments.GetCustomerPayment(customer_id, payment_id);
            return AppResponse.Success(data);
        }
        [HttpGet("pmt/{customer_id}/{payment_id}")]
        public async Task<IActionResult> GetCustomerPayment([FromQuery] long customer_id,string payment_id)
        { 
            var data = await unitOfWork.Payments.GetCustomerPayment(customer_id, payment_id);
            return AppResponse.Success(data);
        }

        [HttpGet("pmt/{customer_id}")]
        public async Task<IActionResult> GetCustomerPayments([FromQuery] long customer_id)
        { 
            var data = await unitOfWork.Payments.GetCustomerPayments(customer_id);
            return AppResponse.Success(data);
        }


        public async static Task<PayPalHttp.HttpResponse> createOrder()
        {
            PayPalHttp.HttpResponse response;
            // Construct a request object and set desired parameters
            // Here, OrdersCreateRequest() creates a POST request to /v2/checkout/orders
            var order = new OrderRequest()
            {
                CheckoutPaymentIntent = "CAPTURE",
                PurchaseUnits = new List<PurchaseUnitRequest>()
                {
                    new PurchaseUnitRequest()
                    {
                        AmountWithBreakdown = new AmountWithBreakdown()
                        {
                            CurrencyCode = "USD",
                            Value = "5.00"
                        }
                    }
                },
                ApplicationContext = new ApplicationContext()
                {
                    ReturnUrl = "https://www.example.com",
                    CancelUrl = "https://www.example.com"
                }
            };


            // Call API with your client and get a response for your call
            var request = new OrdersCreateRequest();
            request.Prefer("return=representation");
            request.RequestBody(order);
            response = await PayPalClient.client().Execute(request);
            var statusCode = response.StatusCode;
            PayPalCheckoutSdk.Orders.Order result = response.Result<PayPalCheckoutSdk.Orders.Order>();
            Console.WriteLine("Status: {0}", result.Status);
            Console.WriteLine("AppZeroAPI.Entities.CustomerOrder Id: {0}", result.Id);
           // Console.WriteLine("Intent: {0}", result.Intent);
            Console.WriteLine("Links:");
            foreach (LinkDescription link in result.Links)
            {
                Console.WriteLine("\t{0}: {1}\tCall Type: {2}", link.Rel, link.Href, link.Method);
            }
            return response;
        }


    }
    public class PayPalClient
    {
        /**
            Setting up PayPal environment with credentials with sandbox cerdentails. 
            For Live, this should be LiveEnvironment Instance. 
         */
        public static PayPalEnvironment environment()
        {
            //sb-qihmh3558739@business.example.com 
            //access_token$sandbox$zztj797p49gk9vbg$228d5e92fcff4cb49f5dae785af5f88e
           
            return new SandboxEnvironment(
                 System.Environment.GetEnvironmentVariable("PAYPAL_CLIENT_ID") != null ?
                 System.Environment.GetEnvironmentVariable("PAYPAL_CLIENT_ID") : "AXsvzo8YSrsScloTKO_Ufg4FXR1f4Le_DA9_j7Htli9qYQrXxTGbolQNueitJvY7ueTSFCR52zX92Nwm",
                 System.Environment.GetEnvironmentVariable("PAYPAL_CLIENT_SECRET") != null ?
                 System.Environment.GetEnvironmentVariable("PAYPAL_CLIENT_SECRET") : "EPmO4vP52YKdD6R0Y1rEq1JDhZPER4qup6T82s45Q2EXO7MZaJ7f5y8v_kW1pr-rbVvWjLxJWZAEaZBr");
            }

        /**
            Returns PayPalHttpClient instance which can be used to invoke PayPal API's.
         */
        public static PayPalHttpClient client()
        {
            return new PayPalHttpClient(environment());
        }

        public static PayPalHttpClient client(string refreshToken)
        {
            return new PayPalHttpClient(environment(), refreshToken);
        }

        /**
            This method can be used to Serialize Object to JSON string.
        */
        public static String ObjectToJSONString(Object serializableObject)
        {
            MemoryStream memoryStream = new MemoryStream();
            var writer = JsonReaderWriterFactory.CreateJsonWriter(
                        memoryStream, Encoding.UTF8, true, true, "  ");
            DataContractJsonSerializer ser = new DataContractJsonSerializer(serializableObject.GetType(), new DataContractJsonSerializerSettings { UseSimpleDictionaryFormat = true });
            ser.WriteObject(writer, serializableObject);
            memoryStream.Position = 0;
            StreamReader sr = new StreamReader(memoryStream);
            return sr.ReadToEnd();
        }
    }
}