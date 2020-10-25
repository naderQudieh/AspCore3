using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AppZeroAPI.Shared;
using Braintree;
using Microsoft.Extensions.Configuration;
using AppZeroAPI.Models;
using AppZeroAPI.Services;

namespace AppZeroAPI.Controllers
{
   
    [Route("api/[controller]")]
    public class CheckoutController : Controller
    {
        public IBraintreeConfiguration config = new BraintreeConfiguration();
        private readonly IBraintreeService _braintreeService;
        public CheckoutController( IBraintreeService braintreeService)
        {
           // _bookService = courseService;
            _braintreeService = braintreeService;
        }
        public IActionResult Create(ProductPurchase model)
        { 

            var gateway = config.GetGateway();

            var request = new TransactionRequest
            {
                
                Amount = Convert.ToDecimal(60),
                PaymentMethodNonce = model.Nonce,
                Customer = new CustomerRequest //Adds customer since they have not booked before
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    Phone = model.PhoneNumber,
                    Id = model.userid
                },
                Options = new TransactionOptionsRequest
                {
                    SubmitForSettlement = true
                },
                BillingAddress = new AddressRequest
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    StreetAddress = model.AddressLine1,
                    ExtendedAddress = model.AddressLine2,
                    Locality = model.City,
                    PostalCode = model.PostCode
                },
            };

            //request.LineItems = cart.CartItems.Select(x => new Braintree.TransactionLineItemRequest
            //{
            //    Name = x.Product.Name,
            //    Description = x.Product.Description,
            //    ProductCode = x.ProductID.ToString(),
            //    Quantity = x.Quantity,
            //    UnitAmount = x.Product.Price,
            //    TotalAmount = x.Product.Price * x.Quantity,
            //    LineItemKind = Braintree.TransactionLineItemKind.DEBIT
            //}).ToArray();
            Result<Transaction> result = gateway.Transaction.Sale(request);
            if (result.IsSuccess())
            {
                
                //EmailDetails(newReservation).Wait();
                
                return View("Success");

            } 
            return View("Failure");
        }

        public IActionResult BraintreePlans()
        {
            var gateway = _braintreeService.GetGateway();
            var plans = gateway.Plan.All();

            return View(plans);
        }

        public IActionResult SubscribeToPlan(string id)
        {
            var gateway = _braintreeService.GetGateway();

            var subscriptionRequest = new SubscriptionRequest()
            {
                PaymentMethodToken = "my-payment-token-value",
                PlanId = id,
            };

            var result = gateway.Subscription.Create(subscriptionRequest);

            if (result.IsSuccess())
            {
                return View("Subscribed");
            }
            return View("NotSubscribed");
        }


        private void test1()
        {
            var gateway = new BraintreeGateway
            {
                Environment = Braintree.Environment.SANDBOX,
                MerchantId = "the_merchant_id",
                PublicKey = "a_public_key",
                PrivateKey = "a_private_key"
            };
            //PaymentMethodNonce = nonceFromTheClient
            TransactionRequest request = new TransactionRequest
            {
                Amount = 1000.00M,
                PaymentMethodNonce =  "",
                Options = new TransactionOptionsRequest
                {
                    SubmitForSettlement = true
                }
            };

            Result<Transaction> result = gateway.Transaction.Sale(request);

            if (result.IsSuccess())
            {
                Transaction transaction = result.Target;
                Console.WriteLine("Success!: " + transaction.Id);
            }
            else if (result.Transaction != null)
            {
                Transaction transaction = result.Transaction;
                Console.WriteLine("Error processing transaction:");
                Console.WriteLine("  Status: " + transaction.Status);
                Console.WriteLine("  Code: " + transaction.ProcessorResponseCode);
                Console.WriteLine("  Text: " + transaction.ProcessorResponseText);
            }
            else
            {
                foreach (ValidationError error in result.Errors.DeepAll())
                {
                    Console.WriteLine("Attribute: " + error.Attribute);
                    Console.WriteLine("  Code: " + error.Code);
                    Console.WriteLine("  Message: " + error.Message);
                }
            }
        }
    }
}
