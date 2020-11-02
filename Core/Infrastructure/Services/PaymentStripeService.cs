using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Stripe;
using Stripe.Checkout;
using AppZeroAPI.Interfaces;
using AppZeroAPI.Shared.PayModel;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace AppZeroAPI.Services
{
    

    public class  PaymentStripeService  
    {
        public decimal ShareAmt { get => 0.8m; }
        private readonly IConfiguration _configuration;
        string ApiKey = "pk_test_51HiQuVAFZv6rpRFk3K1JeutsplKLBU7nFnti3wi6xZ6YW7sHUPJl433JQF4K9kSO0VsxX3edkIgrJrrbdzFPSGdt00a6LlFJ7W";
        string SecretKey = "sk_test_51HiQuVAFZv6rpRFkxiu0mnkJ35QnwdZtPHaXaWaqam4OlEIsLBDB5qphjD9lc38UWwjZwJlrdpd6BvYwLWCzogVu0075iwLofB";
       
        private readonly ILogger<PaymentStripeService> logger;
        private readonly IConfiguration configuration; 
        private HttpContext context;
        public PaymentStripeService(IConfiguration configuration, IHttpContextAccessor contextAccessor, ILogger<PaymentStripeService> logger)
        {
            this.context = contextAccessor.HttpContext;
            this.logger = logger;
            this.configuration = configuration;
            StripeConfiguration.ApiKey = this.SecretKey;// _configuration["Stripe:SecretKey"];
        }


        public async Task<Charge> MakePayment(Stripe.PaymentMethod payModel)
        {
            var response = await Task.FromResult(""); 
            throw new NotImplementedException();
        }
        private async Task<Charge> MakePayment2(PaymentInputModel payModel)
        {
            try
            {
                StripeConfiguration.ApiKey = this.SecretKey; ;// "pk_test_51HiQuVAFZv6rpRFk3K1JeutsplKLBU7nFnti3wi6xZ6YW7sHUPJl433JQF4K9kSO0VsxX3edkIgrJrrbdzFPSGdt00a6LlFJ7W";
                var optionsToken = new TokenCreateOptions
                {
                    Card = new TokenCardOptions
                    {
                        Number = payModel.cardNumber,
                        ExpMonth = payModel.month,
                        ExpYear = payModel.year,
                        Cvc = payModel.cvc
                    },
                };


                var serviceToken = new Stripe.TokenService();
                Token stripeToken = await serviceToken.CreateAsync(optionsToken);

                var options = new ChargeCreateOptions
                {
                    Amount = GetFormattedAmount((long)payModel.value) ,
                    Currency = "USD",
                    Description = $"Payment taken by Moo-In at {payModel.RestaurantName}",
                    Source = stripeToken.Id,
                    Metadata = new Dictionary<string, string>
                    {
                        { "OrderId", payModel.Order.order_id.ToString() },
                    },
                    ApplicationFeeAmount = GetShareAmount(payModel.value)
                };
                var service = new ChargeService();
                Charge charge = await service.CreateAsync(options, new RequestOptions { StripeAccount = payModel.StripeAccountId });

                return charge;
            }
            catch (Exception e)
            {
                throw;
            }
        }

        private Refund Refund(string chargeId, string stripeAccountId)
        {
            var refundService = new RefundService();
            var refundOptions = new RefundCreateOptions()
            {
                Charge = chargeId
            };
            refundOptions.RefundApplicationFee = true;
            Refund refund = refundService.Create(refundOptions, new RequestOptions { StripeAccount = stripeAccountId });
            return refund;
        }
        private async Task<LoginLink> CreateLoginLinkAsync(string accountId, string redirectUrl)
        {
            var options = new LoginLinkCreateOptions
            {
                RedirectUrl = redirectUrl
            };

            var service = new LoginLinkService();
            return await service.CreateAsync(accountId, options);
        }


        private async Task<Customer> CreateCustomerAsync(string token, string name, string email, string phone)
        {
            var options = new CustomerCreateOptions
            {
                Source = token,
                Name = name,
                Email = email,
                Phone = phone
            };

            var service = new CustomerService();
            return await service.CreateAsync(options);
        }

        public async Task<Customer> CreateCustomerAsync(string name, string email, string phone)
        {
            var options = new CustomerCreateOptions
            {
                Name = name,
                Email = email,
                Phone = phone
            };

            var service = new CustomerService();
            return await service.CreateAsync(options);
        }

        public async Task<Customer> UpdateCustomerAsync(string customerId, string name, string email)
        {
            var options = new CustomerUpdateOptions();

            if (!string.IsNullOrEmpty(name))
            {
                options.Name = name;
            }

            if (!string.IsNullOrEmpty(email))
            {
                options.Email = email;
            }

            var service = new CustomerService();
            return await service.UpdateAsync(customerId, options);
        }

        public async Task<StripeList<Card>> GetCardListAsync(string customerId)
        {
            var service = new CardService();
            var options = new CardListOptions
            {
                Limit = 3,
            };
            return await service.ListAsync(customerId, options);
        }


        public async Task<StripeList<PaymentMethod>> GetPaymentMethodsAsync(string customerId)
        {
            var options = new PaymentMethodListOptions
            {
                Customer = customerId,
                Type = "card",
            };

            var service = new PaymentMethodService();
            return await service.ListAsync(options);
        }

        public async Task<Source> AttachAsync(string customerId, string token)
        {
            var options = new SourceAttachOptions
            {
                Source = token,
            };
            var service = new SourceService();
            return await service.AttachAsync(customerId, options);
        }

        public async Task<Charge> CreateChargeAsync(string customerId, string source, string destination, string description, decimal totalAmount, decimal applicationFeeAmount, Dictionary<string, string> metadata)
        {
            var options = new ChargeCreateOptions
            {
                Customer = customerId,
                Source = source,
                Description = description,
                Amount = (long)(totalAmount * 100),
                ApplicationFeeAmount = (long)(applicationFeeAmount * 100),
                TransferData = new ChargeTransferDataOptions { Destination = destination },
                Currency = "usd",
                Capture = false,
                Metadata = metadata
            };

            var service = new ChargeService();
            return await service.CreateAsync(options);
        }

        public async Task<Charge> CreateCaptureAsync(string chargeId, decimal amount, decimal applicationFeeAmount)
        {
            var options = new ChargeCaptureOptions
            {
                Amount = (long)(amount * 100),
                ApplicationFeeAmount = (long)(applicationFeeAmount * 100)
            };

            var service = new ChargeService();
            return await service.CaptureAsync(chargeId, options);
        }

        public async Task<StripeList<Payout>> GetPayoutsAsync(string accountId)
        {
            var payoutService = new PayoutService();

            return await payoutService.ListAsync(new PayoutListOptions { Limit = 100 }, new RequestOptions { StripeAccount = accountId });
        }

        public async Task<List<string>> GetPayoutTransactionsAsync(string accountId, string payoutId)
        {
            var result = new List<string>();

            var balanceTransactionService = new BalanceTransactionService();
            var chargeService = new ChargeService();
            var transferService = new TransferService();

            var requestOptions = new RequestOptions { StripeAccount = accountId };

            var transactions = await balanceTransactionService.ListAsync(new BalanceTransactionListOptions { Payout = payoutId, Type = "payment", Limit = 100 }, requestOptions);
            foreach (var transaction in transactions)
            {
                var payment = await chargeService.GetAsync(transaction.SourceId, null, requestOptions);
                var transfer = await transferService.GetAsync(payment.SourceTransferId);

                result.Add(transfer.SourceTransactionId);
            }

            return result;
        }

        public async Task<Refund> CreateRefundAsync(string chargeId)
        {
            var options = new RefundCreateOptions
            {
                Charge = chargeId,
            };

            var service = new RefundService();
            return await service.CreateAsync(options);
        }

        public async Task<OAuthToken> CreateOAuthTokenAsync(string code)
        {
            var options = new OAuthTokenCreateOptions
            {
                ClientSecret = StripeConfiguration.ApiKey,
                Code = code,
                GrantType = "authorization_code"
            };

            var service = new OAuthTokenService();
            return await service.CreateAsync(options);
        }

        public Subscription Subscribe(string email, string name, string source, string monthlyPlanId, string overagePlanId)
        {
            var customerService = new CustomerService();
            var customer = customerService.Create(new CustomerCreateOptions
            {
                Email = email,
                Description = name,
                Source = source
            });

            var subscriptionService = new SubscriptionService();

            var items = new List<SubscriptionItemOptions> {
                new SubscriptionItemOptions { Plan = monthlyPlanId },
                new SubscriptionItemOptions { Plan = overagePlanId }
            };

            var subscription = subscriptionService.Create(new SubscriptionCreateOptions
            {
                Customer = customer.Id,
                Items = items,
            });

            return subscription;
        }

        public Subscription GetSubscription(string subscriptionId)
        {
            var subscriptionService = new SubscriptionService();

            return subscriptionService.Get(subscriptionId);
        }

        public Invoice GetUpcomingInvoice(string customerId)
        {
            var invoiceService = new InvoiceService();

            var upcomingInvoiceOptions = new UpcomingInvoiceOptions()
            {
                Customer = customerId
            };

            return invoiceService.Upcoming(upcomingInvoiceOptions);
        }

        public UsageRecord AddUsageRecord(string subscriptionItemId, int quantity)
        {
            var usageRecordService = new UsageRecordService();

            var usageRecordOptions = new UsageRecordCreateOptions()
            {
                Quantity = quantity,
                Timestamp = DateTime.UtcNow
            };

            return usageRecordService.Create(subscriptionItemId, usageRecordOptions);
        }

        public Subscription Change(string subscriptionId, string subscriptionItemId, string overageSubscriptionItemId, string monthlyPlanId, string overagePlanId)
        {
            var subscriptionService = new SubscriptionService();

            var items = new List<SubscriptionItemOptions> {
                new SubscriptionItemOptions {
                    Plan = monthlyPlanId,
                },
                new SubscriptionItemOptions {
                    Plan = overagePlanId,
                }
            };

            var options = new SubscriptionUpdateOptions
            {
                CancelAtPeriodEnd = false,
                Items = items,
            };

            return subscriptionService.Update(subscriptionId, options);
        }

        public Subscription CancelSubscription(string stripeSubscriptionId)
        {
            var subscriptionService = new SubscriptionService();

            return subscriptionService.Cancel(stripeSubscriptionId, new SubscriptionCancelOptions { InvoiceNow = true });
        }

        public async Task<Session> CreateSessionAsync(string customerId, string accountId, string itemName, string itemDescription, decimal amount, decimal applicationFee, string successUrl, string cancelUrl)
        {
            var options = new SessionCreateOptions
            {
                Customer = customerId,
                PaymentMethodTypes = new List<string> {
                    "card",
                },
                LineItems = new List<SessionLineItemOptions> {
                    new SessionLineItemOptions {
                        Name = itemName,
                        Description = itemDescription,
                        Amount = (long)(amount * 100),
                        Currency = "usd",
                        Quantity = 1,
                    },
                },
                PaymentIntentData = new SessionPaymentIntentDataOptions
                {
                    Description = itemName,
                    SetupFutureUsage = "off_session",
                    ApplicationFeeAmount = (long)(applicationFee * 100),
                    //TransferData = new Stripe.Checkout.SessionPaymentIntentDataOptions
                    //{
                    //    Destination = accountId,
                    //}
                },
                SuccessUrl = successUrl,
                CancelUrl = cancelUrl
            };

            var service = new SessionService();
            return await service.CreateAsync(options);
        }

        public async Task<Session> GetCheckoutSession(string sessionId)
        {
            var options = new SessionGetOptions();

            options.AddExpand("setup_intent");
            options.AddExpand("payment_intent");

            var service = new SessionService();
            return await service.GetAsync(sessionId, options);
        }

        public async Task<PaymentIntent> CreatePaymentIntentAsync(string customerId, string accountId, string description, decimal amount, decimal applicationFeeAmount, string paymentMethodId)
        {
            var options = new PaymentIntentCreateOptions
            {
                Description = description,
                Amount = (long)(amount * 100),
                ApplicationFeeAmount = (long)(applicationFeeAmount * 100),
                TransferData = new PaymentIntentTransferDataOptions
                {
                    Destination = accountId,
                },
                Currency = "usd",
                PaymentMethodTypes = new List<string>
                {
                    "card"
                },
                Customer = customerId,
                PaymentMethod = paymentMethodId,
                Confirm = true,
                OffSession = true
            };

            var service = new PaymentIntentService();
            return await service.CreateAsync(options);         
        }

        public async Task<Customer> GetCustomerAsync(string customerId)
        {
            var service = new CustomerService();

            return await service.GetAsync(customerId);
        }

        public async Task<Account> GetAccountAsync(string accountId)
        {
            var service = new AccountService();

            return await service.GetAsync(accountId);
        }

        public async Task<PaymentIntent> GetPaymentIntentAsync(string paymentIntentId)
        {
            var service = new PaymentIntentService();

            return await service.GetAsync(paymentIntentId);
        }

        public async Task<PaymentMethod> GetPaymentMethodAsync(string paymentMethodId)
        {
            var service = new PaymentMethodService();

            return await service.GetAsync(paymentMethodId);
        }

        public async Task<PaymentMethod> DetachPaymentMethodAsync(string paymentMethodId)
        {
            var service = new PaymentMethodService();

            return await service.DetachAsync(paymentMethodId);
        }

        private long GetShareAmount(decimal amountToPay)
        {
            decimal shareAmt = decimal.Round((amountToPay * ShareAmt / 100), 2);
            long formattedShareAmt = GetFormattedAmount(shareAmt);
            return formattedShareAmt;
        }

        private static long GetFormattedAmount(decimal AmountToProcess)
        {
            return Convert.ToInt64(decimal.Round(AmountToProcess, 2) * 100);
        }
    }
}
