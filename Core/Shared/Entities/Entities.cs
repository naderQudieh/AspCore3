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
        [Key]
        public Int32? Id { get; set; }

        public DateTime CreatedDate { get; set; }

        public Int32? CreatedBy { get; set; }


        public Int32? ModifiedBy { get; set; }

        public DateTime ModifiedDate { get; set; }
    }
    
 
    public class MailTemplates : BaseEntity
    {
        public MailTemplates()
        {

        }
        public MailTemplates(Int32? id, string name, string description, string templateData, string subject
            , int? createdBy, int? modifiedBy, DateTime createdDate, DateTime modifiedDate, int templateType)
        {
            this.Id = id;
            this.Name = name;
            this.Description = description;
            this.TemplateData = templateData;
            this.Subject = subject;
            this.CreatedBy = createdBy;
            this.ModifiedBy = modifiedBy;
            this.CreatedDate = createdDate;
            this.ModifiedDate = modifiedDate;
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
    public class LookUps
    {
        public string code { get; set; }
        public string value { get; set; }
    }

    [Table("customer_carts")]
    public class CustomerCart
    {
        public string cart_id { get; set; }
        public long customer_id { get; set; }
        public decimal cart_total { get; set; }
        public decimal cart_discount { get; set; }
        public decimal total_payable { get; set; }
        public string cart_status { get; set; }

        public DateTime date_created { get; set; }
        public DateTime date_modified { get; set; }
        public Customer customer { get; set; }
        public List<CartItem> cartItems { get; set; }
    }

    [Table("customer_cart_items")]
    public class CartItem
    {
        public string cart_id { get; set; }
        public long product_id { get; set; }
        public decimal qty { get; set; }
        public decimal price { get; set; }
        public decimal discount { get; set; }
        public decimal price_to_pay { get; set; }

        public string discount_description { get; set; }
        public DateTime date_created { get; set; }
        public DateTime date_modified { get; set; }
        public virtual Product Product { get; set; }
    }
    [Table("customers")]
    public class Customer
    {
        public long customer_id { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string email { get; set; }
        public string contact_phone { get; set; }
        public string contact_mobile { get; set; }
        public DateTime date_created { get; set; }
        public DateTime date_modified { get; set; }
    }
    [Table("customer_order_items")]
    public class CustomerOrderItem
    {
        public long order_id { get; set; }
        public long product_id { get; set; }
        public long qty { get; set; }
        public decimal price { get; set; }
        public decimal discount { get; set; }
        public decimal total_payable { get; set; }

        public string discount_description { get; set; }
        public DateTime date_created { get; set; }
        public DateTime date_modified { get; set; }
        public virtual Product Product { get; set; }
    }
    [Table("customer_orders")]
    public class CustomerOrder
    {
        public long order_id { get; set; }
        public long customer_id { get; set; }
        public long shipping_address_id { get; set; }
        public long payment_id { get; set; }
        public decimal order_total { get; set; }
 
        public decimal discount_amount { get; set; }
        public decimal total_payable { get; set; }
        public string order_status { get; set; }
        public OrderStatus Status { get; set; } = OrderStatus.Draft;
        public string confirm_no { get; set; }
        public string PaypalToken { get; set; }
        public DateTime date_created { get; set; }
        public DateTime date_modified { get; set; }
        public string PaymentProviderSessionId { get; set; }
        public virtual List<CustomerOrderItem> orderItems { get; set; }
        public virtual Customer customer { get; set; }
        public virtual PaymentProviderType PaymentProvider { get; set; }
        
        //public Address ship_to { get; set; }
        //public Address bill_to { get; set; }


    }
    public class Address
    {
        public long address_id { get; set; }
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
        public DateTime date_created { get; set; }
        public DateTime date_modified { get; set; }
    }
    
    public class PurchaseData
    {
        public string payment_method_nonce;
        public string client_token;
        public decimal amount;
    }
    [Table("customer_cards")]
    public class CustomerCreditCard
    {
        [Key]
        [Required]
        [Column("card_id")]
        [JsonProperty("card_id")]
        public string card_id { get; set; }
        public string cardtype { get; set; }
        public long customer_id { get; set; }
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
        public DateTime date_created { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "date_modified")]
        public DateTime date_modified { get; set; }

    }

    [Table("customers_bank")]
    public class CustomerBank
    {
        public long customer_id { get; set; }
        public string bank_routing_number { get; set; }
        public string bank_account_number { get; set; }
        public string bank_check_number { get; set; } 
        public string bank_name { get; set; } 
        public Address billing_address { get; set; }  
        public DateTime date_created { get; set; } 
        public DateTime date_modified { get; set; }

    }
    [Table("customer_payments")]
    public class Payment
    {
        [Key]
        [Required]
        [Column("payment_id")]
        [JsonProperty("payment_id")]
        public string payment_id { get; set; }
        public long customer_id { get; set; }
        public int order_id { get; set; }
        public PaymentMethod payment_method { get; set; } 
        public CustomerCreditCard cardused { get; set; }
        public CustomerBank bankused { get; set; }
        public long amount { get; set; }
        public string payment_status { get; set; }
        public DateTime payment_date { get; set; }
        public string currency { get; set; }
        public string reference_id { get; set; }
        public string response_code { get; set; }
        public DateTime date_created { get; set; }
        public DateTime date_modified { get; set; }

    }



    [Table("user_tokens")]
    public class UserTokenData
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
    public class UserProfile
    {
        [Dapper.Contrib.Extensions.Key]
        [Key]
        [Required]
        [Column("user_id")]
        [JsonProperty("user_id")]
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
    public class UserToken
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


    [Table("products")]
    public class Product
    {
        [Dapper.Contrib.Extensions.Key]
        [Column("product_Id")]
        [JsonProperty("product_Id")]
        public long product_Id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string barcode { get; set; }
        public string imge_url { get; set; }
        public int qty_in_stock { get; set; }
        public float unit_price { get; set; }
        public int department_id { get; set; }
        public DateTime date_created { get; set; }
        public DateTime date_modified { get; set; }

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
