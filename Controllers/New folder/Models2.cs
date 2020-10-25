using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;

namespace AppZeroAPI.Models
{
    public class TransactionDTO
    {

        public int TransactionId { get; set; }
        public string ExternalId { get; set; }
        public string TransactionType { get; set; }
        public long Amount { get; set; }
        public string Status { get; set; }
        public string Metadata { get; set; }
        public DateTime TransactionTime { get; set; }
        public string UserId { get; set; }
        public int OrderId { get; set; }
        public int VendorId { get; set; }
        public string Instrument { get; set; }
        public string Response { get; set; }
    }
    public class OrderStatus
    {
        public short Id { get; set; }
        public string Name { get; set; }
    }

    public class OrderStatusConfiguration : IEntityTypeConfiguration<OrderStatus>
    {
        public void Configure(EntityTypeBuilder<OrderStatus> builder)
        {
            builder.HasData(new OrderStatus() { Id = 1, Name = "Pending" });
            builder.HasData(new OrderStatus() { Id = 2, Name = "Paid" });
        }
    }
    public class Order
    {
        public long Id { get; set; }
        public long CustomerId { get; set; }
        public Customer Customer { get; set; }

        public short OrderStatusId { get; set; }
        public OrderStatus OrderStatus { get; set; }

        public decimal TotalPrice { get; set; }
        public decimal Discount { get; set; }
        public decimal TotalPayable { get; set; }
        public decimal PaidAmount { get; set; }
        public DateTime UpdatedTime { get; set; }
        public DateTime PaidTime { get; set; }

        public List<OrderItem> OrderItems { get; set; }
        public List<OrderPayment> OrderPayments { get; set; }
    }
    public class OrderPayment
    {
        public long Id { get; set; }
        public long OrderId { get; set; }
        public Order Order { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentTime { get; set; }
        public string PaymentMethod { get; set; }
    }
    [DataContract]
    public class AccessToken
    {
        [DataMember(Name = "scope")]
        public string Scope { get; set; }

        [DataMember(Name = "nonce")]
        public string Nonce { get; set; }

        [DataMember(Name = "access_token")]
        public string AccessTokenString { get; set; }

        [DataMember(Name = "token_type")]
        public string TokenType { get; set; }

        [DataMember(Name = "app_id")]
        public string AppId { get; set; }

        [DataMember(Name = "expires_in")]
        public string ExpiresIn { get; set; }
    }
    [DataContract]
    public class ClientInfo
    {
        [DataMember(Name = "client_account")]
        public string ClientAccount { get; set; }

        [DataMember(Name = "client_id")]
        public string ClientId { get; set; }

        [DataMember(Name = "client_secret")]
        public string ClientSecret { get; set; }
    }
    public class Address
    {
        public int ID { get; set; }
        public Customer Owner { get; set; }
        public string Recipient { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string Country { get; set; }
        public string Phone { get; set; }
        public AddressType Type { get; set; }
    }

    public enum AddressType
    {
        Billing,
        Shipping
    }
    public class Order3
    {
        public List<OrderItem> OrderItems { get; set; }
        public Customer Customer { get; set; }
        public long CustomerId { get; set; }
        public int ID { get; set; }
        public Customer Purchaser { get; set; }
        public DateTime DateOrdered { get; set; }
        public Address ShipTo { get; set; }
        public Address BillTo { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Shipping { get; set; }
        public decimal Total { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal Discount { get; set; }
        public decimal TotalPayable { get; set; }
        //public OrderStatus OrderStatus { get; set; }
        public DateTime UpdatedTime { get; set; }
    }

    
    public class OrderDetail
    {
        public int ID { get; set; }
        public Customer Purchaser { get; set; }
        public Product Item { get; set; }
        public DateTime PlacedInCart { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Shipping { get; set; }
        public bool CheckedOut { get; set; }
        public Order Order { get; set; }

        public decimal ExtendedPrice
        {
            get
            {
                return Quantity * UnitPrice;
            }

        }

        public decimal TotalPrice
        {
            get
            {
                return ExtendedPrice + Shipping;
            }
        }
    }
    public class ProductVM
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public decimal Shipping { get; set; }
        
        public string Category { get; set; }
    }
    public class ShoppingCart
    {
        public Order _order;
        public List<OrderDetail> _shoppingCart { get; set; }
        public string payeeEmail { get; set; }
        public decimal TotalExtendedPrice
        {
            get
            {
                return _shoppingCart.Sum(p => p.ExtendedPrice);
            }
        }

        public decimal TotalShipping
        {
            get
            {
                return _shoppingCart.Sum(p => p.Shipping);
            }
        }

        public decimal GrandTotal
        {
            get
            {
                return TotalExtendedPrice + TotalShipping;
            }
        }

        public ShoppingCart()
        {
            _shoppingCart = new List<OrderDetail>();
            _order = new  Order();
        }

        public List<OrderDetail> GetItems()
        {
            // TODO: return clone to prevent writing to the data outside the class?
            return _shoppingCart;
        }

        public Order GetOrder()
        {
            // TODO: return clone to prevent writing to the data outside the class?
            return _order;
        }

        public void AddOrderDetail(OrderDetail newItem)
        {
            _shoppingCart.Add(newItem);
        }

        public void AddProduct(Product product)
        {
            OrderDetail orderDetail = _shoppingCart.Find(p => p.Item.ID == product.ID);
            if (orderDetail == null)
            {
                orderDetail = new OrderDetail()
                {
                    Item = product,
                    PlacedInCart = DateTime.Now,
                    Quantity = 1,
                    UnitPrice = product.Price,
                   // Shipping = product.Shipping
                };
                _shoppingCart.Add(orderDetail);
            }
            else
            {
                orderDetail.Quantity++;
            }
        }

        public void DecrementProduct(Product product)
        {
            OrderDetail orderDetail = _shoppingCart.Find(p => p.Item.ID == product.ID);
            if (!(orderDetail == null) && orderDetail.Quantity > 0)
            {
                orderDetail.Quantity--;
            }
            if (orderDetail.Quantity == 0)
            {
                RemoveProduct(product);
            }
        }

        public void RemoveProduct(Product product)
        {
            OrderDetail orderDetail = _shoppingCart.Find(p => p.Item.ID == product.ID);
            if (!(orderDetail == null))
            {
                _shoppingCart.Remove(orderDetail);
            }
        }



    }
    public class OrderPaymentRequest
    {
        public long OrderId { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; }
    }
    public class Customer
    {
        public long Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public long UserId { get; set; }

        public User User { get; set; }

        public string CurrentAddress { get; set; }
        public string BillingAddress { get; set; }

    }
    public class OrderItem
    {
        public long Id { get; set; }
        public long OrderId { get; set; }
        public Order Order { get; set; }
        public long ProductId { get; set; }
        public Product Product { get; set; }
        public decimal Price { get; set; }
    }
    
    public class CustomerDetails
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string CountryCode { get; set; }

        public string CountryName { get; set; }


        public string UserName { get; set; }
    }
    public class OrderRequest
    {
        public List<long> ProductIds { get; set; }
    }
    public class PayRequest
    {
        [Required]
        [RegularExpression(@"^[a-zA-Z]+(([',. -][a-zA-Z ])?[a-zA-Z]*)*$", ErrorMessage = "Invalid Name format")]
        public string CardHolderName { get; set; }

        [Required]
        [RegularExpression(@"^\d{13,19}$", ErrorMessage = "Invalid card number format")]
        public string Cardnumber { get; set; }
        [Required]
        [RegularExpression(@"^((0[1-9])|(1[0-2]))[\/](([2-9][0-9]))$", ErrorMessage = "Invalid expiry date format. Should be MM/YY")]
        public string ExpiryDate { get; set; }
        [Required]
        [RegularExpression(@"^\d{3}$", ErrorMessage = "Invalid CVV.  Should be 3 digits only")]
        public int Cvv { get; set; }
        [Required]
        public string CardType { get; set; }
        [Required]
        [RegularExpression(@"^[A-Za-z]{3}$", ErrorMessage = "Invalid Currency format.  Should be 3 alphabets only")]
        public string Currency { get; set; }
        [Required]
        public double Amount { get; set; }
    }
    public class BankResponse
    {
        public int Status { get; set; }
        public String Identifier { get; set; }
    }
    public partial class Merchant
    {
        public Merchant()
        {
            Payment = new HashSet<Payment>();
        }

        public int Merchantid { get; set; }
        public string Apikey { get; set; }

        public virtual ICollection<Payment> Payment { get; set; }
    }
    public class PaymentDetails
    {
        public string Paymentid { get; set; }
        public int? Merchantid { get; set; }
        public string Cardnumber { get; set; }
        public string Cardholdername { get; set; }
        public string Cardtype { get; set; }
        public string Expirydate { get; set; }
        public int? Status { get; set; }
        public double? Amount { get; set; }
        public DateTime Paymentdate { get; set; }
        public string Currency { get; set; }


        public PaymentDetails(string paymentId, int? merchantId, string cardNumber, string cardHolderName, string cardType, string expiryDate, int? status, double? amount, DateTime paymentDate, string currency)
        {
            Cardnumber = cardNumber;
            Cardholdername = cardHolderName;
            Cardtype = cardType;
            Expirydate = expiryDate;
            Status = status;
            Merchantid = merchantId;
            Paymentid = paymentId;
            Amount = amount;
            Paymentdate = paymentDate;
            Currency = currency;
        }

    }
    public partial class Payment
    {
        public string Paymentid { get; set; }
        public int? Merchantid { get; set; }
        public string Cardnumber { get; set; }
        public string Cardholdername { get; set; }
        public string Cardtype { get; set; }
        public string Expirydate { get; set; }
        public int? Status { get; set; }
        public double? Amount { get; set; }
        public DateTime Paymentdate { get; set; }
        public string Currency { get; set; }

        public virtual Merchant Merchant { get; set; }
    }
    public class Review
    {
        public int ID { get; set; }
        public string UserID { get; set; }
        public int Rating { get; set; }
        public string Text { get; set; }
        public int ProductID { get; set; }
        public Product Product { get; set; }
    }
    public class ProductColor
    {
        public int ID { get; set; }
        public string Color { get; set; }
        public int ProductID { get; set; }
        public Product Product { get; set; }
    }
    public class ProductSize
    {
        public int ID { get; set; }
        public string Size { get; set; }
        public int ProductID { get; set; }
        public Product Product { get; set; }
    }
    public class ProductImage
    {
        public int ID { get; set; }
        public string Url { get; set; }
        public string AltText { get; set; }
        public int ProductID { get; set; }
        public Product Product { get; set; }
    }
    public class Category
    {
        public Category()
        {
            this.Products = new HashSet<Product>();
        }
        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string BannerImageUrl { get; set; }
        public ICollection<Product> Products { get; set; }
    }
    public class Product
    {
        public Product()
        {
            this.ProductImages = new HashSet<ProductImage>();
            this.ProductSizes = new HashSet<ProductSize>();
            this.ProductColors = new HashSet<ProductColor>();
            this.Reviews = new HashSet<Review>();
        }

        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int CategoryID { get; set; }
        public Category Category { get; set; }
        public ICollection<ProductImage> ProductImages { get; set; }
        public ICollection<ProductSize> ProductSizes { get; set; }
        public ICollection<ProductColor> ProductColors { get; set; }
        public ICollection<Review> Reviews { get; set; }
    }
    public class CartLineModel
    {
       
        public int Quantity { get; set; }
    }
    public class CartModel
    {
        public CartModel()
        {
            Items = new List<CartLineModel>();
        }

        public List<CartLineModel> Items { get; set; }
    }
    public class CartItem
    {
        public int ID { get; set; }

        public int Quantity { get; set; }

        public int ProductID { get; set; }
        public Product Product { get; set; }

        public ProductColor ProductColor { get; set; }

        public ProductSize ProductSize { get; set; }
    }
    public class Cart
    {
        public Cart()
        {
            this.CartItems = new HashSet<CartItem>();
        }

        public int ID { get; set; }
        public Guid CookieIdentifier { get; set; }

     
        public ICollection<CartItem> CartItems { get; set; }
    }
    public class ProductPurchase
    {
        public string userid { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string ContactName { get; set; }
        public string Nonce { get; set; }
        public string ProductId { get; set; }
        public int ProductQty { get; set; }
        public float TotalValue { get; set; } 
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string City { get; set; }
        public string PostCode { get; set; }
     
        public string ShippingRegion { get; set; }
       
        public string ShippingCountry { get; set; }
    } 
}


