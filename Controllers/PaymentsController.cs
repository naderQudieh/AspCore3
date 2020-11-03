using System.IO;
using System.Threading.Tasks;
using AppZeroAPI.Entities;
using AppZeroAPI.Models;
using AppZeroAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Stripe;

namespace AppZeroAPI.Controllers
{
    [ApiController]
    [Route("api/pay")]
    public class PaymentsController : BaseController
    {
        public string SiteBaseUrl = "";
        string ApiKey = "pk_test_51HiQuVAFZv6rpRFk3K1JeutsplKLBU7nFnti3wi6xZ6YW7sHUPJl433JQF4K9kSO0VsxX3edkIgrJrrbdzFPSGdt00a6LlFJ7W";
        string SecretKey = "sk_test_51HiQuVAFZv6rpRFkxiu0mnkJ35QnwdZtPHaXaWaqam4OlEIsLBDB5qphjD9lc38UWwjZwJlrdpd6BvYwLWCzogVu0075iwLofB";
        private readonly ILogger<PaymentsController> _logger;
        private readonly IPaymentService _paymentService;


        // Stripe webhook signing secret
        private const string WhSecret = "whsec_Q1YOT6Zmq31ke1WoPYSizgND4sLpm7Ax";
        public PaymentsController(IPaymentService paymentService, ILogger<PaymentsController> logger)
        {
            _paymentService = paymentService;
            _logger = logger;
        }

        //[Authorize]
        [HttpPost("{cartId}")]
        public async Task<ActionResult<CustomerCart>> CreateOrUpdatePaymentIntent(string cartId)
        {
            var basket = await _paymentService.CreateOrUpdatePaymentIntent(cartId);

            if (basket == null) return BadRequest(new { code = 400, message = "Problem with your basket"});
             
            return AppResponse.Success(basket);
            
        }

        [HttpPost("webhook")]
        public async Task<ActionResult> StripeWebhook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

            var stripeEvent = EventUtility.ConstructEvent(json, Request.Headers["Stripe-Signature"], WhSecret);

            PaymentIntent intent;
            Order order;

            switch (stripeEvent.Type)
            {
                case "payment_intent.succeeded":
                    intent = (PaymentIntent)stripeEvent.Data.Object;
                    _logger.LogInformation("Payment Succeeded: ", intent.Id);
                   // order = await _paymentService.UpdateOrderPaymentSucceeded(intent.Id);
                    //_logger.LogInformation("Order updated to payment received: ", order.Id);
                    break;

                case "payment_intent.payment_failed":
                    intent = (PaymentIntent)stripeEvent.Data.Object;
                    _logger.LogInformation("Payment Failed: ", intent.Id);
                    //order = await _paymentService.UpdateOrderPaymentFailed(intent.Id);
                   // _logger.LogInformation("Payment failed: ", order.Id);
                    break;
            }

            return new EmptyResult();

        }

    }
}