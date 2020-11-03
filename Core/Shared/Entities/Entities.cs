using AppZeroAPI.Shared.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppZeroAPI.Entities
{
    
    public class BaseEntity
    {
        
        [Dapper.Contrib.Extensions.ExplicitKey] 
        [System.ComponentModel.DataAnnotations.KeyAttribute] 
        [Column("rec_id")]
        [JsonProperty("rec_id")]
        public string rec_id { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "date_created")]
        public DateTime date_created { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "date_modified")] 
        public DateTime date_modified { get; set; }
        public BaseEntity()
        {
            rec_id = Guid.NewGuid().ToString().ToLower().Replace("-","");
        }
    }

    public class LogData : BaseEntity
    {

        public int Id { get; set; }

        public string Category { get; set; }

        public string Message { get; set; }

        public string User { get; set; }

        public int UserId { get; set; }
        public DateTimeOffset? MessageOn { get; set; }
    }
    public class MailTemplates : BaseEntity
    {
        public MailTemplates()
        {

        }
        public MailTemplates(Int32? id, string? rec_id, string name, string description, string templateData, string subject
            , int? createdBy, int? modifiedBy, DateTime createdDate, DateTime modifiedDate, int templateType)
        {
            
            this.Name = name;
            this.Description = description;
            this.TemplateData = templateData;
            this.Subject = subject; 
            this.date_created = createdDate;
            this.date_modified = modifiedDate;
            this.TemplateType = templateType;
        }
        public string Name { get; set; }
        public string Description { get; set; }
        public string TemplateData { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public string Subject { get; set; }
        public int TemplateType { get; set; }
    }
    public class LookUps:BaseEntity
    {
        public string code { get; set; }
        public string value { get; set; }
    }

    [Table("customer_carts")]
    public class CustomerCart : BaseEntity
    {
        
        public string customer_id { get; set; }
        public decimal cart_total { get; set; }
        public decimal cart_discount { get; set; }
        public decimal total_payable { get; set; }
        public string cart_status { get; set; } 
        public string paymentIntentId { get; set; }
        public int? deliveryMethodId { get; set; }
        
        [NotMapped]
        [Dapper.Contrib.Extensions.Write(false)] 
        public string client_secret { get; set; }

        [NotMapped]
        [Dapper.Contrib.Extensions.Write(false)] 
        public string stripeToken { get; set; }
        [NotMapped]
        [Dapper.Contrib.Extensions.Write(false)] 
        public string stripeEmail { get; set; }

        [NotMapped]
        [Dapper.Contrib.Extensions.Write(false)]
        public Customer customer { get; set; }
        [NotMapped]
        [Dapper.Contrib.Extensions.Write(false)]
        public List<CartItem> cartItems { get; set; }

    }

    [Table("customer_cart_items")]
    public class CartItem : BaseEntity
    {
        public string cart_id { get; set; }
        public string product_id { get; set; }
        public int qty { get; set; }
        public decimal price { get; set; }
        public decimal discount { get; set; }
        public decimal total_payable { get; set; }

        public string discount_description { get; set; }
      
        [NotMapped]
        [Dapper.Contrib.Extensions.Write(false)]
        public virtual Product Product { get; set; }
 
    }
    [Table("customers")]
    public class Customer : BaseEntity
    {  
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string email { get; set; }
        public string contact_phone { get; set; }
        public string contact_mobile { get; set; } 
 
    }
    [Table("customer_order_items")]
    public class CustomerOrderItem : BaseEntity
    {  
        public string order_id { get; set; }
        public string product_id { get; set; }
        public int qty { get; set; }
        public decimal price { get; set; }
        public decimal discount { get; set; }
        public decimal total_payable { get; set; }

        public string discount_description { get; set; }
        [NotMapped]
        [Dapper.Contrib.Extensions.Write(false)]
        public virtual Product Product { get; set; }

       
    }
    [Table("customer_orders")]
    public class CustomerOrder : BaseEntity
    {   
        public string customer_id { get; set; }
        public long shipping_address_id { get; set; }
        public string payment_id { get; set; }
        public decimal order_total { get; set; }
 
        public decimal discount_amount { get; set; }
        public decimal total_payable { get; set; }
        public string order_status { get; set; }
        public OrderStatus Status { get; set; } = OrderStatus.Draft;
        public string confirm_no { get; set; }

        public OrderPaymentStatus order_pymnt_status { get; set; } = OrderPaymentStatus.NotKnown;

        public string PaymentProviderSessionId { get; set; }

        [NotMapped]
        [Dapper.Contrib.Extensions.Write(false)]
        public virtual List<CustomerOrderItem> orderItems { get; set; }

        [NotMapped]
        [Dapper.Contrib.Extensions.Write(false)]
        public virtual Customer customer { get; set; }
        public virtual PaymentProviderType PaymentProvider { get; set; }
        [NotMapped]
        [Dapper.Contrib.Extensions.Write(false)]
        public string paypal_token { get; set; }
        [NotMapped]
        [Dapper.Contrib.Extensions.Write(false)]
        public string paymentIntentId { get; set; }
        //public Address ship_to { get; set; }
        //public Address bill_to { get; set; }


    }

    [Table("products")]
    public class Product : BaseEntity
    { 
        public string name { get; set; }
        public string description { get; set; }
        public string barcode { get; set; }
        public string imge_url { get; set; }
        public int qty_in_stock { get; set; }
        public decimal unit_price { get; set; }
        public int department_id { get; set; }

    }

    [Table("customer_payments")]
    public class Payment : BaseEntity
    {

        public string card_id { get; set; }
        public string customer_id { get; set; }
        public string order_id { get; set; }
        public PaymentMethod payment_method { get; set; }
        public long amount { get; set; }
        public string payment_status { get; set; }
        public DateTime payment_date { get; set; }
        public string currency { get; set; }
        public string reference_id { get; set; }
        public string response_code { get; set; }

        //public CustomerCreditCard cardused { get; set; }
        //public CustomerBank bankused { get; set; }
    } 
   
    [Table("customer_cards")]
    public class CustomerCreditCard : BaseEntity
    { 
        public string card_id { get; set; }
        public string cardtype { get; set; }
        public string customer_id { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string card_holder_name { get; set; }
        public string card_number { get; set; }  
        public int card_exp_mm { get; set; }
        public int card_exp_yy { get; set; }
        public string card_cvv { get; set; } 
        public string card_status { get; set; }
        public string address_id { get; set; }
        public string address1 { get; set; }
        public string address2 { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string zip { get; set; }
        public string zip4 { get; set; }
        public string country { get; set; }
        public string phone { get; set; }
        public string mobile { get; set; }
     

    }

    [Table("customers_bank")]
    public class CustomerBank : BaseEntity
    {
        public string customer_id { get; set; }
        public string bank_routing_number { get; set; }
        public string bank_account_number { get; set; }
        public string bank_check_number { get; set; } 
        public string bank_name { get; set; } 
        public Address billing_address { get; set; }  
        

    }

    public class Address : BaseEntity
    {
        public string addressid { get; set; }
        public string address_id { get; set; }
        public string address1 { get; set; }
        public string address2 { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string zip { get; set; }
        public string zip4 { get; set; }
        public string country { get; set; }
        public string phone { get; set; }
        public string mobile { get; set; }
        public string type { get; set; }


    }

    public class PurchaseData
    {
        public string payment_method_nonce;
        public string client_token;
        public decimal amount;
    }

    [Table("user_tokens")]
    public class UserTokenData : BaseEntity
    {
        [Column("token_id")]
        public int TokenId { get; set; }

        [Column("user_id")]
        public long user_id { get; set; }



        [Column("access_token")]
        public string AccessToken { get; set; }

        [Column("refresh_token")]
        public string RefreshToken { get; set; }

        [Column("black_listed")]
        public bool BlackListed { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("access_token_expires_at")]
        public DateTime AccessTokenExpiresAt { get; set; }

        [Column("refresh_token_expires_at")]
        public DateTime RefreshTokenExpiresAt { get; set; }

        [Column("created_by_ip")]
        public string CreatedByIP { get; set; }

        [NotMapped]
        public string ReplacedByToken { get; set; }
        [NotMapped]
        public bool IsActive => !BlackListed && !IsExpired;

        [NotMapped]
        public bool IsExpired => DateTime.UtcNow >= AccessTokenExpiresAt;
        public UserTokenData()
        {

        }
        public UserTokenData(string token, DateTime expires)
        {
            this.AccessToken = token;
            this.AccessTokenExpiresAt = expires;
        }
        public UserTokenData(string token, string refreshToken, DateTime expires)
        {
            this.AccessToken = token;
            this.RefreshToken = refreshToken;
            this.AccessTokenExpiresAt = expires;
        }
    }





    public class UserTokensData
    {

        public string userId { get; set; }
        public string email { get; set; }

        IList<UserToken> tokens { get; set; }
    }

    [Table("user_profiles")]
    public class UserProfile : BaseEntity
    {
       
        public long user_id { get; set; }

        [Column("username")]
        [JsonProperty("username")]
        public string username { get; set; }
        public string fname { get; set; }
        public string lname { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        [JsonIgnore]
        public string password { get; set; }
        [JsonIgnore]
        public string password_hash { get; set; }

        [JsonIgnore]
        public string password_salt { get; set; }
        public string role { get; set; }


        [NotMapped]
        public string verification_token { get; set; }
        [NotMapped]
        public DateTime? date_verified { get; set; }

          
        public DateTime date_created { get; set; }

         
        public DateTime date_modified { get; set; }

        [Column("language")]
        public int language { get; set; }

        [Column("profile_picture")]
        public string profile_picture { get; set; }
       

    }

    [Table("user_refresh_tokens")]
    public class UserToken : BaseEntity
    {
        
        [JsonProperty("user_id")]
        public long UserId { get; set; }

        [JsonProperty("id_token")]
        public long TokenId { get; set; }

        [Required]
        [Column("access_token")]
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }
        [Required]
        [Column("refresh_token")]
        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; }
        public bool BlackListed { get; set; }
        [JsonProperty("created_at")]
        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("expires_at")]
        [JsonProperty("expires_at")]
        public DateTime ExpiresAt { get; set; }

        [Column("is_active")]
        [JsonProperty("is_active")]
        public bool IsActive { get; set; }

       
    }


  
    public class Detect
    {
        [Dapper.Contrib.Extensions.Key]
        public long Id { get; set; }

        public string DeviceType { get; set; }

        public string Os { get; set; }

        public long? UserId { get; set; }

        public string Browser { get; set; }

        public string UserIp { get; set; }

    }

}
