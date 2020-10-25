
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BrownNews.Services
{
    public class PayPalService : IPayPalService
    {
        public readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _context;
        private readonly IUrlHelper _urlHelper;

        public PayPalService(IConfiguration configuration, IHttpContextAccessor context, IUrlHelper urlHelper)
        {
            _configuration = configuration;
            _context = context;
            _urlHelper = urlHelper;
        }

        public async Task<string> ProcessPayment(PayPalTestViewModel model)
        {
            var environment = new SandboxEnvironment(_configuration["PayPal:SandBox:ClientId"], _configuration["PayPal:SandBox:ClientSecret"]);
            var client = new PayPalHttpClient(environment);

            var payment = new PayPal.v1.Payments.Payment()
            {
                Intent = "sale",
                Transactions = new List<Transaction>()
                    {
                        new Transaction()
                        {
                            Amount = new Amount()
                            {
                                Total = model.Amount,
                                Currency = "INR"
                            }
                        }
                    },
                RedirectUrls = new RedirectUrls()
                {
                    ReturnUrl = _urlHelper.Action("Done", "Test", new { amt = model.Amount }, _context.HttpContext.Request.Scheme, _context.HttpContext.Request.Host.ToString()),
                    CancelUrl = _urlHelper.Action("Cancel", "Test", new { amt = model.Amount }, _context.HttpContext.Request.Scheme, _context.HttpContext.Request.Host.ToString())
                },
                Payer = new Payer()
                {
                    PaymentMethod = "paypal"
                }
            };

            PaymentCreateRequest request = new PaymentCreateRequest();
            request.RequestBody(payment);

            System.Net.HttpStatusCode statusCode;

            BraintreeHttp.HttpResponse response = await client.Execute(request);
            statusCode = response.StatusCode;
            Payment result = response.Result<Payment>();

            string redirectUrl = null;
            foreach (LinkDescriptionObject link in result.Links)
            {
                if (link.Rel.Equals("approval_url"))
                {
                    redirectUrl = link.Href;
                }
            }

            if (redirectUrl == null)
            {
                // Didn't find an approval_url in response.Links
                return null;
            }
            else
            {
                return redirectUrl;
            }
        }
    }
}
