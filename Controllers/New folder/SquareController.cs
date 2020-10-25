using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Square;
using Square.Exceptions;
using Square.Models;
using AppZeroAPI.Shared;
using Microsoft.Extensions.Options;
using Square.Apis;
using AppZeroAPI.Models;

namespace AppZeroAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SquareController : ControllerBase
    {
         private readonly AppSettings appSettings;
       

        public SquareController( IOptions<AppSettings> appSettings  )
        {
            
            this.appSettings = appSettings?.Value;
        }
        private string NewIdempotencyKey()
        {
            return Guid.NewGuid().ToString();
        }
        [HttpPost("process-payment")]
        public async Task<IActionResult> ProcessPaymentAsync(string nonce)
        {
            try
            {
                Square.Environment environment = appSettings.Environment == "sandbox" ?
                Square.Environment.Sandbox : Square.Environment.Production;

                // Build base client
                SquareClient client = new SquareClient.Builder()
                    .Environment(environment)
                    .AccessToken(this.appSettings.AccessToken)
                    .Build();

                IPaymentsApi PaymentsApi = client.PaymentsApi;
                CreatePaymentRequest request_body = new CreatePaymentRequest(nonce, this.NewIdempotencyKey(), new Money(100, "USD"));

                CreatePaymentResponse responce = await PaymentsApi.CreatePaymentAsync(request_body);

                if (responce?.Payment?.Status == "COMPLETED")
                {
                    //this.UpdateCart(new CartModel());
                    return this.Ok();
                }
                else
                {
                    return this.BadRequest($"STATUS: {responce?.Payment?.Status}");
                }
            }
            catch (Exception ex)
            {
                return this.BadRequest($"PaymentError: { ex.Message }");
            }
        }

        /*
          public (Payment, string) CreatePayment(CreatePaymentRequest paymentRequest, int storeId)
          {
              if (paymentRequest == null)
                  throw new ArgumentNullException(nameof(paymentRequest));

              var client = CreateSquareClient(storeId);

              try
              {
                  var paymentResponse = client.PaymentsApi.CreatePayment(paymentRequest);
                  if (paymentResponse == null)
                      throw new NopException("No service response");

                  ThrowErrorsIfExists(paymentResponse.Errors);

                  return (paymentResponse.Payment, null);
              }
              catch (Exception exception)
              {
                  return (null, CatchException(exception));
              }
          }
          public (bool, string) CompletePayment(string paymentId, int storeId)
          {
              if (string.IsNullOrEmpty(paymentId))
                  return (false, null);

              var client = CreateSquareClient(storeId);

              try
              {
                  var paymentResponse = client.PaymentsApi.CompletePayment(paymentId, null);
                  if (paymentResponse == null)
                      throw new NopException("No service response");

                  ThrowErrorsIfExists(paymentResponse.Errors);

                  //if there are no errors in the response, payment was successfully completed
                  return (true, null);
              }
              catch (Exception exception)
              {
                  return (false, CatchException(exception));
              }
          }
          public (bool, string) CancelPayment(string paymentId, int storeId)
          {
              if (string.IsNullOrEmpty(paymentId))
                  return (false, null);

              var client = CreateSquareClient(storeId);

              try
              {
                  var paymentResponse = client.PaymentsApi.CancelPayment(paymentId);
                  if (paymentResponse == null)
                      throw new NopException("No service response");

                  ThrowErrorsIfExists(paymentResponse.Errors);

                  //if there are no errors in the response, payment was successfully canceled
                  return (true, null);
              }
              catch (Exception exception)
              {
                  return (false, CatchException(exception));
              }
          }
          public (PaymentRefund, string) RefundPayment(RefundPaymentRequest refundPaymentRequest, int storeId)
          {
              if (refundPaymentRequest == null)
                  throw new ArgumentNullException(nameof(refundPaymentRequest));

              var client = CreateSquareClient(storeId);

              try
              {
                  var refundPaymentResponse = client.RefundsApi.RefundPayment(refundPaymentRequest);
                  if (refundPaymentResponse == null)
                      throw new NopException("No service response");

                  ThrowErrorsIfExists(refundPaymentResponse.Errors);

                  return (refundPaymentResponse.Refund, null);
              }
              catch (Exception exception)
              {
                  return (null, CatchException(exception));
              }
          }
          public (string AccessToken, string RefreshToken) ObtainAccessToken(string authorizationCode, int storeId)
          {
              return _squareAuthorizationHttpClient.ObtainAccessTokenAsync(authorizationCode, storeId).Result;
          }
          public (string AccessToken, string RefreshToken) RenewAccessToken(int storeId)
          {
              return _squareAuthorizationHttpClient.RenewAccessTokenAsync(storeId).Result;
          }
          public bool RevokeAccessTokens(int storeId)
          {
              return _squareAuthorizationHttpClient.RevokeAccessTokensAsync(storeId).Result;
          }  */
        private void Main()
        {
            SquareClient client = new SquareClient.Builder()
                .Environment(Square.Environment.Sandbox)
                .AccessToken("YOUR_SANDBOX_ACCESS_TOKEN")
                .Build();

            var api = client.LocationsApi;

            try
            {
                var locations = api.ListLocations().Locations;
                Console.WriteLine("Successfully called ListLocations");
                // Your business logic here
            }
            catch (ApiException e)
            {
                var errors = e.Errors;
                var statusCode = e.ResponseCode;
                var httpContext = e.HttpContext;
                Console.WriteLine("ApiException occurred:");
                Console.WriteLine("Headers:");
                foreach (var item in httpContext.Request.Headers)
                {
                    //Display all the headers except Authorization
                    if (item.Key != "Authorization")
                    {
                        Console.WriteLine("\\t{0}: \\{1}", item.Key, item.Value);
                    }
                }
                Console.WriteLine("Status Code: \\{0}", statusCode);
                foreach (Error error in errors)
                {
                    Console.WriteLine("Error Category:{0} Code:{1} Detail:{2}", error.Category, error.Code, error.Detail);
                }

                // Your error handling code
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception occurred");
                // Your error handling code
            }
        }
    }
}
    
