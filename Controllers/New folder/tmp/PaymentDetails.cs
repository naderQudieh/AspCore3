using AppZeroAPI.Entities;
using AppZeroAPI.Interfaces;
using AppZeroAPI.Models;
using AppZeroAPI.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Mime;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Text;
using Microsoft.AspNetCore.Hosting;

namespace AppZeroAPI.Controllers
{
    [ApiController]
    //[AuthorizeUser] 
    [Route("api/PaymentDetails")]
    public class PaymentDetailsController : BaseController
    {
        private IWebHostEnvironment _env;
        private readonly ILogger<PaymentDetailsController> logger;
        private readonly IUnitOfWork unitOfWork;
        private readonly IPaymentService paymentService;
        public PaymentDetailsController(IWebHostEnvironment _env,
            IUnitOfWork unitOfWork, IPaymentService paymentService,
            ILogger<PaymentDetailsController> logger)
        {
            this._env = _env;
            this.logger = logger; 
            this.paymentService = paymentService;
            this.unitOfWork = unitOfWork;
        }

        
        [HttpPost]
        public async Task<string> CreateOrder()
        {

            // Get shopping cart from session
            ShoppingCart shoppingCart = null;// HttpContext.Session.GetObjectFromJson<ShoppingCart>("_shopping_cart");
            if (shoppingCart.GetOrder().ShipTo == null)
            {
                shoppingCart.GetOrder().ShipTo = new Address();
            }
            shoppingCart.GetOrder().ShipTo.Country = "US";

            PayPalApiClient paypalClient = new PayPalApiClient();

            // Get client id from paypal.json
            ClientInfo paypalSecrets = paypalClient.GetClientSecrets(_env.ContentRootPath);

            // Get access token
            AccessToken accessToken = await paypalClient.GetAccessToken(paypalSecrets);

            // Create JSON string with order information
            shoppingCart.payeeEmail = paypalSecrets.ClientAccount;
            string orderData = paypalClient.CreateOrder(shoppingCart);

            // Post order to PayPal API and return order ID to front end
            return await paypalClient.PostOrder(orderData, accessToken.AccessTokenString);
        }


        [HttpPost]
        public async Task<IActionResult> ExecutePayment(string paymentId, IFormCollection data)
        {
            PayPalApiClient paypalClient = new PayPalApiClient();

            // Get client id from paypal.json
            ClientInfo paypalSecrets = paypalClient.GetClientSecrets(_env.ContentRootPath);

            // Get access token
            AccessToken accessToken = await paypalClient.GetAccessToken(paypalSecrets);

            string payerId = Request.Form["PayerID"];

            string uri = "https://api.sandbox.paypal.com/v1/payments/payment/" + paymentId + "/execute";
            string postData = JsonConvert.SerializeObject(new
            {
                payer_id = payerId
            });
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken.AccessTokenString);
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, uri);
                request.Content = new StringContent(postData, Encoding.UTF8, "application/json");
                var response = await client.SendAsync(request);

                var result = response.Content.ReadAsStringAsync().Result;
                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException(result.ToString());
                }
                var orderId = new JsonDeserializer(result).GetString("id");
            }
            return Ok();
        }

        [HttpGet("{payID}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PaymentDetails>> GetPayment(string payID)
        {
            int merchantID =   Authenticate();
            if (merchantID == -3)
            {
                return new ObjectResult(HttpStatusCode.ExpectationFailed);

            }
            if (merchantID < 0)
            {
                return new ObjectResult(HttpStatusCode.Unauthorized);
            }
            PaymentDetails paymentDetails;

            try
            {
                paymentDetails =   unitOfWork.Payment.getPaymentDetails(merchantID, payID);
                return AppResponse.Success(paymentDetails);
                // paymentDetails = await _paymentService.getPaymentDetails(merchantID, payID);
                // return Ok(paymentDetails);
            }
            catch (Exception ex)
            {
                return Unauthorized(ex.Message);
            }


        }



        // POST: api/Payments
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<BankResponse>> PostPaymentToBank(PayRequest payReq)
        {
            int merchantID =   Authenticate();
            if (merchantID == -3)
            {
                return BadRequest("An error occured.");
            }
            if (merchantID < 0)
            {
                return Unauthorized();
            }


            if (payReq == null)
            {
                return BadRequest();
            }


            try
            {
                CardValidator.validateCard(payReq);
                payReq.Cardnumber = payReq.Cardnumber.Replace(" ", "");
                BankResponse bankResponse = new BankResponse();
               // bankResponse =   unitOfWork.Payment.getPaymentDetails(merchantID, "");
                bankResponse = await paymentService.makePayment(payReq);
                Payment payment = new Payment()
                {
                    Status = bankResponse.Status,
                    Paymentid = bankResponse.Identifier,
                    Merchantid = merchantID,
                    Amount = payReq.Amount,
                    Expirydate = payReq.ExpiryDate,
                    Cardholdername = payReq.CardHolderName,
                    Cardnumber = payReq.Cardnumber,
                    Cardtype = payReq.CardType,
                    Currency = payReq.Currency
                };
                  paymentService.addPaymentDetails(payment);
                if (bankResponse.Status == 1000)
                {
                    return Ok(bankResponse);
                }
                return BadRequest(bankResponse);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

        }


        private  int  Authenticate()
        {
            string key = Request.Headers["ApiKey"];
            if (key == null)
            {
                return -2;
            }
            try
            {
                return paymentService.authenticateApiKey(key);
            }
            catch (Exception)
            {
                return -3;
            }

        }
    }

    public class CardValidator
    {

        public static void validateCard(PayRequest payReq)
        {

            if (!checkExpiryDate(payReq.ExpiryDate))
            {
                throw new Exception("Invalid expiry date or card expired!");
            }
        }

        private static bool checkExpiryDate(string expDate)
        {
            string date = "01/" + expDate;
            DateTime dt1 = DateTime.Parse(date);
            DateTime dt2 = DateTime.Now;
            //if (dt1.Date < dt2.Date)
            //{
            //    return false;
            //}


            if ((dt1.Year - dt2.Year) >= 0 && (dt1.Year - dt2.Year) <= 3 && dt1.Month >= dt2.Month)
            {
                return true;
            }

            return false;
        }

    }
}
