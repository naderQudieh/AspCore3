using AppZeroAPI.Entities;
using AppZeroAPI.Interfaces;
using AppZeroAPI.Models;
using AppZeroAPI.Repository;
using AppZeroAPI.Shared;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AppZeroAPI.Services
{
    public class CustomerService : BaseRepository, ICustomerService
    {
        public string SiteBaseUrl = "";
        string ApiKey = "pk_test_51HiQuVAFZv6rpRFk3K1JeutsplKLBU7nFnti3wi6xZ6YW7sHUPJl433JQF4K9kSO0VsxX3edkIgrJrrbdzFPSGdt00a6LlFJ7W";
        string SecretKey = "sk_test_51HiQuVAFZv6rpRFkxiu0mnkJ35QnwdZtPHaXaWaqam4OlEIsLBDB5qphjD9lc38UWwjZwJlrdpd6BvYwLWCzogVu0075iwLofB";

    
        private readonly ILogger<CustomerService> logger; 
        private readonly IMapper _mapper; 
        private readonly IUnitOfWork  unitOfWork;

        public CustomerService(IConfiguration configuration, ILogger<CustomerService> logger,
            IUnitOfWork unitOfWork,   IMapper mapper) : base(configuration)
        {
            _mapper = mapper;
            this.unitOfWork = unitOfWork;
            this.logger = logger;
            
        }


        public async Task<bool> UpdatePaymentStatus(string payment_id, string payment_status)
        {
            return await unitOfWork.Payments.UpdatePaymentStatus(payment_id, payment_status);
        }
        public async Task<IEnumerable<CustomerOrder>> GetOrdersForCustomer(string customer_id)
        {
            return await unitOfWork.Orders.GetOrdersForCustomer(customer_id);
        }
        public async Task<CustomerOrder> GetOrderDetails(string order_id)
        {
            return await unitOfWork.Orders.GetOrderDetails(order_id,true);
        }

        public async Task<CustomerCart> GetCartForCustomer(string customer_id)
        {
            return await unitOfWork.Carts.GetCustomerCartDetails(customer_id, false);
        }
        public async Task<CustomerCart> GetCartDetailsForCustomer(string customer_id)
        {
            return await unitOfWork.Carts.GetCustomerCartDetails(customer_id, true);
        }
        public async Task<bool> UpdateCart(CustomerCart cart)
        {
            return await unitOfWork.Carts.UpdateAsync(cart);
        }
        public async Task<CustomerCart> GetCartDetails(string cart_id)
        {
            return await unitOfWork.Carts.GetCartDetails(cart_id, true);
        }

        public async Task<int> AddCartDetails(CustomerCart customerCart)
        {
            return await unitOfWork.Carts.AddDetailsAsync(customerCart);
        }

    }
}
