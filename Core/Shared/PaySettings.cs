using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Stripe.Checkout;
using Stripe.Infrastructure ;
using AppZeroAPI.Shared.Enums;
using System.Runtime.Serialization;
using AppZeroAPI.Entities;
using System.ComponentModel.DataAnnotations;

namespace AppZeroAPI.Shared.PayModel
{
    public class StripeClientModel
    {
        public string Email { get; set; }
        public string Name { get; set; }
        public string CardNumber { get; set; }
        public int ExpMonth { get; set; }
        public int ExpYear { get; set; }
        public string CVV { get; set; }
    }
    public class PayModel
    {
        [Required]
        [Display(Name = "Cardholder Name")]
        public string  Name { get; set; }

        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Address")]
        public string Address { get; set; }

        [Required]
        [Display(Name = "Card Number")]
        public string CardNumder { get; set; }

        [Required]
        [Display(Name = "Expiration Month")]
        public int ExpMonth { get; set; }

        [Required]
        [Display(Name = "Expiration Year")]
        public int ExpYear { get; set; }

        [Required]
        public string CVC { get; set; }

        [Required]
        public int Amount { get; set; }

        public Product[] Products { get; set; }
        
        public string StripeToken { get; set; }

        public string OrderName { get; set; }
        public string OrderDescription { get; set; }

        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string AddressCity { get; set; }
        public string AddressState { get; set; }
        public string AddressZip { get; set; }
    }
    
    public class PaymentModel
    {
        public string cardnumber { get; set; }
        public int month { get; set; }
        public int year { get; set; }
        public string cvc { get; set; }
        public int value { get; set; }
    }




    public abstract class BasicRequestModel
    {
    }
    public class StripeCreditCardRequestModel
    {
        public string CardHolderName { get; set; }
        public string cardNumber { get; set; }
        public long? ExpiryYear { get; set; }
        public long? ExpiryMonth { get; set; }
        public string cardCVV { get; set; }
        public string cardHolderAddress { get; set; }
        public string cardHolderPostalCode { get; set; }
    }
    public class CreditCardDataModel
    {
        public string CardHolderName { get; set; }
        public string cardNumber { get; set; }
        public int cardExpiryMonth { get; set; }
        public int cardExpiryYear { get; set; }
        public string cardCVV { get; set; }
        public string cardHolderAddress { get; set; }
        public string cardHolderPostalCode { get; set; }
        public string GetCardF4L4()
        {
            if (!string.IsNullOrWhiteSpace(cardNumber) && cardNumber.Length == 16)
            {
                return cardNumber.Substring(0, 4) + cardNumber.Substring(cardNumber.Length - 4, 4);
            }

            return string.Empty;
        }

        public string GetCardL4()
        {
            if (!string.IsNullOrWhiteSpace(cardNumber) && cardNumber.Length == 16)
            {
                return cardNumber.Substring(cardNumber.Length - 4, 4);
            }

            return string.Empty;
        }
    }
    public class PaymentDataModel
    {
        public PaymentType PaymentType { get; set; }
        public PaymentGateWayType PaymentGateWay { get; set; }
        public CurrencyType CurrencyType { get; set; }
        public PaymentStatusType PaymentStatusType { get; set; }
        public int InvoiceId { get; set; }
        public decimal PaymentAmount { get; set; }
        public string CheckNo { get; set; }
        public string TokenId { get; set; }
        public string TransactionId { get; set; }
        public string Note { get; set; }
        public CreditCardDataModel CreditCard { get; set; }
    }
    public class StripeBasicRequestModel : BasicRequestModel
    {
        public CurrencyType Currency { get; set; }
        public bool Test { get; set; }

        public string TransactionId { get; set; }
        public decimal Amount { get; set; }
        public string OrderNumber { get; set; }
        public string TokenId { get; set; }
        public string CardF4L4 { get; set; }
        public string Description { get; set; }
        public StripeCreditCardRequestModel CreditCard { get; set; }

        public static implicit operator StripeBasicRequestModel(PaymentDataModel source)
        {
            return new StripeBasicRequestModel
            {
                Amount = source.PaymentAmount,
                Currency = source.CurrencyType,
                Description = source.Note,

                TokenId = source.TokenId,
                CreditCard = source.CreditCard != null ? new StripeCreditCardRequestModel
                {
                    cardCVV = source.CreditCard.cardCVV,
                    ExpiryYear = source.CreditCard.cardExpiryYear,
                    ExpiryMonth = source.CreditCard.cardExpiryMonth,
                    cardHolderAddress = source.CreditCard.cardHolderAddress,
                    CardHolderName = source.CreditCard.CardHolderName,
                    cardHolderPostalCode = source.CreditCard.cardHolderPostalCode,
                    cardNumber = source.CreditCard.cardNumber
                } : null
            };
        }
    }


    [DataContract(Name = "result_model")]
    public class PaymentResultApiModel
    {
        [DataMember(Name = "transaction_id")]
        public string TransactionId { get; set; }

        [DataMember(Name = "success")]
        public bool Success { get; set; }

        [DataMember(Name = "message")]
        public string Message { get; set; }

        [DataMember(Name = "approved")]
        public bool Approved { get; set; }

        [DataMember(Name = "auth_code")]
        public string AuthCode { get; set; }
        [DataMember(Name = "card_last4")]
        public string CardLast4 { get; set; }
        [DataMember(Name = "amount_paid")]
        public decimal AmountPaid { get; set; }
        [DataMember(Name = "amount_paid_total")]
        public decimal AmountPaidTotal { get; set; }
        [DataMember(Name = "failure_code")]
        public string FailureCode { get; set; }
        [DataMember(Name = "failure_message")]
        public string FailureMessage { get; set; }

    }
    public class PaymentInputModel
    {
        public string cardNumber { get; set; }

        public int month { get; set; }

        public int year { get; set; }

        public string cvc { get; set; }

        public decimal value { get; set; }


        public AppZeroAPI.Entities.CustomerOrder Order { get; set; }
        public string StripeAccountId { get; set; }
        public string RestaurantName { get; set; }
    }
    public class CreateSubscriptionRequest
    {
        [JsonProperty("paymentMethodId")]
        public string PaymentMethod { get; set; }

        [JsonProperty("customerId")]
        public string Customer { get; set; }

        [JsonProperty("priceId")]
        public string Price { get; set; }

        [JsonProperty("Quantity")]
        public int Quantity { get; set; }
    }
    public class RetryInvoiceRequest
    {
        [JsonProperty("customerId")]
        public string Customer { get; set; }

        [JsonProperty("paymentMethodId")]
        public string PaymentMethod { get; set; }

        [JsonProperty("invoiceId")]
        public string Invoice { get; set; }
    }
    public class CreateCustomerResponse
    {
        [JsonProperty("customer")]
        public Customer Customer { get; set; }
    }

    public static class StripeOptions
    {
        public static string Environment { get; set; } = "production";
        public static string PublishableKey { get; set; } = "pk_test_51HiQuVAFZv6rpRFk3K1JeutsplKLBU7nFnti3wi6xZ6YW7sHUPJl433JQF4K9kSO0VsxX3edkIgrJrrbdzFPSGdt00a6LlFJ7W";// Environment.GetEnvironmentVariable("STRIPE_PUBLISHABLE_KEY");
        public static string SecretKey { get; set; } = "sk_test_51HiQuVAFZv6rpRFkxiu0mnkJ35QnwdZtPHaXaWaqam4OlEIsLBDB5qphjD9lc38UWwjZwJlrdpd6BvYwLWCzogVu0075iwLofB";// Environment.GetEnvironmentVariable("STRIPE_SECRET_KEY");
        public static string WebhookSecret { get; set; } = "";// Environment.GetEnvironmentVariable("STRIPE_WEBHOOK_SECRET");
        public static string Price { get; set; } = "";//Environment.GetEnvironmentVariable("PRICE");
        public static string Domain { get; set; } = "";// Environment.GetEnvironmentVariable("DOMAIN");
  

    }
    public class PaypalConfiguration
    {
        // env?: 'production' | 'sandbox';
        public string Environment { get; set; } = "production";
        public string ClientIdSandbox { get; set; }
        public string ClientIdProduction { get; set; }
        public string SecretSandbox { get; set; }
        public string SecretProduction { get; set; }

    }
   
    

    public class CreateCustomerRequest
    {
        [JsonProperty("email")]
        public string Email { get; set; }
    }
 

    public class ConfigResponse
    {
        [JsonProperty("publicKey")]
        public string PublishableKey { get; set; }

        [JsonProperty("unitAmount")]
        public long? UnitAmount { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }
    }
}
