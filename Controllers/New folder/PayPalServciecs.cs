using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PayPal.Api.OpenIdConnect;
 using PayPal.Api;
using PayPal.Util;

namespace AppZeroAPI.Core.Infrastructure.Services
{
    public class PaypalConfiguration
    {
        // env?: 'production' | 'sandbox';
        public string Environment { get; set; } = "production";
        public string ClientIdSandbox { get; set; }
        public string ClientIdProduction { get; set; }
        public string SecretSandbox { get; set; }
        public string SecretProduction { get; set; }

    }
    public class PayPalPaymentProvider
    {
        private readonly PaypalConfiguration configuration;

        private readonly OrderService orderService;

        public PayPalPaymentProvider(OrderService orderService, PaypalConfiguration configuration)
        {
            this.orderService = orderService;
            this.configuration = configuration;

        }

        public override async Task<IOrder> ChargeAsync(PayPalChargeBindings bindings, CancellationToken cancellationToken = default(CancellationToken))
        {
            var environment = configuration.Environment == "production" ?
                new SandboxEnvironment(configuration.ClientIdProduction, configuration.SecretProduction) :
                new SandboxEnvironment(configuration.ClientIdSandbox, configuration.SecretSandbox);

            var client = new PayPalHttpClient(environment);

            var request = new PaymentExecuteRequest(bindings.PaymentId);
            request.RequestBody(new PaymentExecution()
            {
                PayerId = bindings.PayerId
            });

            try
            {
                var response = await client.Execute(request);
                var statusCode = response.StatusCode;
                var result = response.Result<PayPalPayment>();

                var order = await this.orderService.GetByIdAsync(bindings.OrderId, cancellationToken);
                var payments = new List<OurPayment>();
                foreach (var transaction in result.Transactions)
                {
                    if (!Decimal.TryParse(transaction.Amount.Total, out decimal amount))
                    {
                        amount = 0;
                    }

                    var payment = new OurPayment()
                    {
                        Title = "Paypal Payment",
                        Provider = Name,
                        Reference = $"{bindings.PaymentId}",
                        Status = PaymentStatus.Paid,
                        Date = DateTime.UtcNow,
                        Method = PaymentMethod.Electronic,
                        Details = $"Payment Order #{order.Reference}: {transaction.Amount.Total} {transaction.Amount.Currency}",
                        Currency = transaction.Amount.Currency,
                        Amount = amount
                    };
                    payments.Add(payment);
                    await orderService.AddPayment(order, payment, cancellationToken);
                }
                return order;

            }
            catch (HttpException httpException)
            {
                var statusCode = httpException.StatusCode;
                var debugId = httpException.Headers.GetValues("PayPal-Debug-Id").FirstOrDefault();
                // TODO: better error text
                throw new AppException($"impossible to create payment, statusCode: '{statusCode}' paypal debug Id '{debugId}'");
            }

        }

    }
}
