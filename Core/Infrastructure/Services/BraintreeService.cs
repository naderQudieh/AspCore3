using AppZeroAPI.Entities;
using AppZeroAPI.Interfaces;
using Braintree;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppZeroAPI.Services
{
   
    public class BraintreeService  
    {
        private readonly IConfiguration _config;
        public BraintreeService(IConfiguration config)
        {
            _config = config;
        }
        public IBraintreeGateway GetBraintreeGateway()
        {
            return CreateBraintreeGateway();
        }
        public IBraintreeGateway CreateBraintreeGateway()
        {
            var newGateway = new BraintreeGateway()
            {
                Environment = Braintree.Environment.SANDBOX,
                MerchantId = _config.GetValue<string>("BraintreeGateway:MerchantId"),
                PublicKey = _config.GetValue<string>("BraintreeGateway:PublicKey"),
                PrivateKey = _config.GetValue<string>("BraintreeGateway:PrivateKey"),
            };
            return newGateway;
        }

        public  async Task<Result<Braintree.Transaction>> Sale(CustomerCreditCard card, AppZeroAPI.Entities.Address address)
        {


            var gateway = CreateBraintreeGateway();
            var saleRequest = new TransactionRequest
            {
                Amount = 20.00M,
                PaymentMethodNonce = "fake-valid-nonce",
                Options = new TransactionOptionsRequest
                {
                    SubmitForSettlement = true
                },
                CreditCard = new Braintree.TransactionCreditCardRequest
                {
                    CardholderName = card.card_holder_name,
                    CVV = card.card_cvv,
                    ExpirationMonth = card.card_exp_mm.ToString(),
                    ExpirationYear = card.card_exp_yy.ToString(),
                    Number = card.card_number
                },
                BillingAddress = new Braintree.AddressRequest
                {
                    StreetAddress = address.address1,
                    PostalCode = address.zip,
                    Region = address.state,
                    //Locality = address.BillingCity,
                    CountryName = "USA"

                }
            };

            Result<Transaction> result = await gateway.Transaction.SaleAsync(saleRequest);
            return result;
        }


        public  async Task<Result<Transaction>> Refund(string transactionId)
        {

            var gateway = CreateBraintreeGateway();
            Result<Braintree.Transaction> result = await gateway.Transaction.RefundAsync(transactionId, new TransactionRefundRequest { });
            return result;
        }

       
        public async Task<string> GetClientToken(string username)
        {
            try
            {

                var gateway = CreateBraintreeGateway();
                var createReq = new CustomerRequest
                {
                    CustomerId = username
                };

                var custResponse = gateway.Customer.Create(createReq);

                var customer = gateway.Customer.Find(custResponse.Target.Id);

                //If this fails and say "CustomerId" is not found, call it with no customerID
                var clientToken = await gateway.ClientToken.GenerateAsync(new ClientTokenRequest
                {
                    CustomerId = customer.Id
                });


                return clientToken;
            }
            catch (Exception ex)
            {

                return ex.Message;
            }

        }


        public  async Task<string> GetCustomerId(string email, string phone, string firstName, string lastName)
        {
            var gateway = CreateBraintreeGateway();
            Braintree.CustomerSearchRequest search = new Braintree.CustomerSearchRequest();
            search.Email.Is(email);
            var searchResult = await gateway.Customer.SearchAsync(search);
            if (searchResult.Ids.Any())
            {
                return searchResult.FirstItem.Id;
            }
            else
            {
                Braintree.CustomerRequest customer = new Braintree.CustomerRequest();
                customer.Email = email;
                customer.Phone = phone;
                customer.FirstName = firstName;
                customer.LastName = lastName;
                var customerResult = await gateway.Customer.CreateAsync(customer);
                return customerResult.Target.Id;
            }
        }

    }
}
