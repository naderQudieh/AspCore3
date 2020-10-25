using Braintree;
using Microsoft.AspNetCore.Mvc;
using PaypalNETCore.Config;

namespace PaypalNETCore.Controllers
{
    [Route("api/[controller]")]
    public class CheckoutsController : Controller
    {
        private readonly IBraintreeConfiguration _config = new BraintreeConfiguration();

        [HttpGet]
        public string Get()
        {
            var gateway = _config.GetGateway();
            var clientToken = gateway.ClientToken.generate();
            return clientToken;
        }

        [HttpPost]
        public void Create(string amount, string nonce)
        {
            var gateway = _config.GetGateway();
            decimal _amount = 10;

            var _nonce = nonce;
            var request = new TransactionRequest
            {
                Amount = _amount,
                PaymentMethodNonce = _nonce
            };

            gateway.Transaction.Sale(request);
        }
    }
}
