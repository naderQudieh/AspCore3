using Braintree;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Text.Json.Serialization;
using JsonConverter = Newtonsoft.Json.JsonConverter;
using JsonConverterAttribute = Newtonsoft.Json.JsonConverterAttribute;

namespace AppZeroAPI.Shared
{
    public class JsonDeserializer
    {
        private IDictionary<string, object> jsonData { get; set; }

        public JsonDeserializer(string json)
        {
            jsonData = JsonConvert.DeserializeObject<IDictionary<string, object>>(json);
        }

        public string GetString(string path)
        {
            return (string)GetObject(path);
        }

        public int? GetInt(string path)
        {
            int? result = null;

            object o = GetObject(path);
            if (o == null)
            {
                return result;
            }

            if (o is string)
            {
                result = Int32.Parse((string)o);
            }
            else
            {
                result = (Int32)o;
            }

            return result;
        }

        public object GetObject(string path)
        {
            object result = null;

            var curr = jsonData;
            var paths = path.Split('.');
            var pathCount = paths.Count();

            try
            {
                for (int i = 0; i < pathCount; i++)
                {
                    var key = paths[i];
                    if (i == (pathCount - 1))
                    {
                        result = curr[key];
                    }
                    else
                    {
                        curr = (IDictionary<string, object>)curr[key];
                    }
                }
            }
            catch
            {
                // Probably means an invalid path (ie object doesn't exist)
            }

            return result;
        }
    }
    public static class SessionExtensions
    {
        public static void SetObjectAsJson(this ISession session, string key, object value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }

        public static T GetObjectFromJson<T>(this ISession session, string key)
        {
            var value = session.GetString(key);

            return value == null ? default(T) : JsonConvert.DeserializeObject<T>(value);
        }
    }
    public class AppSettings
    {
        public string Environment { get; set; }
        public string ApplicationId { get; set; }
        public string AccessToken { get; set; }
        public string LocationId { get; set; }
    }
    public interface IBraintreeConfiguration
    {
        IBraintreeGateway CreateGateway();
        string GetConfigurationSetting(string setting);
        IBraintreeGateway GetGateway();
    }
    public class BraintreeConfiguration : IBraintreeConfiguration
    {
        public string Environment { get; set; }
        public string MerchantId { get; set; }
        public string PublicKey { get; set; }
        public string PrivateKey { get; set; }
        private IBraintreeGateway BraintreeGateway { get; set; }

        public IBraintreeGateway CreateGateway()
        {
            Environment = System.Environment.GetEnvironmentVariable("BraintreeEnvironment");
            MerchantId = System.Environment.GetEnvironmentVariable("BraintreeMerchantId");
            PublicKey = System.Environment.GetEnvironmentVariable("BraintreePublicKey");
            PrivateKey = System.Environment.GetEnvironmentVariable("BraintreePrivateKey");

            if (MerchantId == null || PublicKey == null || PrivateKey == null)
            {
                Environment = GetConfigurationSetting("BraintreeEnvironment");
                MerchantId = GetConfigurationSetting("BraintreeMerchantId");
                PublicKey = GetConfigurationSetting("BraintreePublicKey");
                PrivateKey = GetConfigurationSetting("BraintreePrivateKey");
            }

            return new BraintreeGateway(Environment, MerchantId, PublicKey, PrivateKey);
        }

        public string GetConfigurationSetting(string setting)
        {
            return ConfigurationManager.AppSettings[setting];
        }

        public IBraintreeGateway GetGateway()
        {
            if (BraintreeGateway == null)
            {
                BraintreeGateway = CreateGateway();
            }

            return BraintreeGateway;
        }
    }
    public enum Status
    {
        InActive,
        Active
    }
    public enum BillStatus
    {
        [DescriptionAttribute("New")]
        New,
        [DescriptionAttribute("In progress")]
        InProgress,
        [DescriptionAttribute("Returned")]
        Returned,
        [DescriptionAttribute("Cancelled")]
        Cancelled,
        [DescriptionAttribute("Completed")]
        Completed
    }
    public enum PaymentMethod
    {
        [DescriptionAttribute("Cash on delivery")]
        CashOnDelivery,
        [DescriptionAttribute("Online banking")]
        OnlineBanking,
        [DescriptionAttribute("Payment gateway")]
        PaymentGateway,
        [DescriptionAttribute("Visa")]
        Visa,
        [DescriptionAttribute("Master card")]
        MasterCard,
        [DescriptionAttribute("Paypal")]
        PayPal,
        [DescriptionAttribute("Atm")]
        Atm
    }

    public static class Langauge
    {
        public const int Arabic = 1;
        public const int English = 2;
        public const int Spanish = 3;
        public const int French = 4;
    }
    public static class Role
    {
        public const string Admin = "Admin";
        public const string User = "User";
        public const string Client = "Client";
    }
    public enum AuthType
    {
        Simple,
        Email,
        Phone
    }

    public enum TokenType : int
    {
        [Description("None")]
        None = 0,

        [Description("Bearer")]
        Bearer = 1
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum TokenStatus : int
    {
        [Description("None")]
        None = 0,

        [Description("Active")]
        Active = 1,

        [Description("Terminate")]
        Terminate = 2,

        [Description("Pending")]
        Pending = 3
    }
    public enum TokenConfiguration
    {
        Issuer,
        Audience,
        AccessSecret,
        RefreshSecret,
        AccessExpire,
        RefreshExpire,
        Secret,
        Enabled
    }
    public class EmailConfiguration
    {
        public string From { get; set; }
        public string SmtpServer { get; set; }
        public int Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool IsSSLEnable { get; set; }
        public bool IsEnabled { get; set; }
    }
    public class JwtOptions
    {
        public string JwtId { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public string Subject { get; set; }
        public DateTime IssuedAt => DateTime.UtcNow;
        public DateTime NotBefore => DateTime.UtcNow;
        public string SecretKey { get; set; }
        public int TokenExpireInMints { get; set; }
        public int RefreshExpireInDays { get; set; }
        public int JwtClockSkew { get; set; }
        public bool IsEnabled => true;
    }

}