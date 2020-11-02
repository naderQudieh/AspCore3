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
    [Route("api/paybtree")]
    public class TransBraintreeController : BaseController
    {
        private readonly ILogger<TransBraintreeController> logger;
        private readonly IUnitOfWork unitOfWork;
        private readonly PaymentBraintreeService braintreeService;

        public TransBraintreeController(PaymentBraintreeService braintreeService, IUnitOfWork unitOfWork, ILogger<TransBraintreeController> logger)
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
            PayPalCheckoutSdk.Orders.Order  result = response.Result<PayPalCheckoutSdk.Orders.Order>();
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
     
}