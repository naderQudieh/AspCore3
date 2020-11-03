using AppZeroAPI.Entities;
using AppZeroAPI.Interfaces;
using AppZeroAPI.Models;
using AppZeroAPI.Services;
using AppZeroAPI.Shared;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;

namespace AppZeroAPI.Controllers
{
    //[AuthorizeUser]
    [ApiController]
    [Route("api/customers")]
    public class CustomerController : BaseController
    {
        private readonly IMapper  mapper;
        private readonly ILogger<CustomerController> logger;
        private readonly ICustomerService customerService;


        public CustomerController(ICustomerService customerService,  ILogger<CustomerController> logger,  IMapper mapper)
        {
            this.mapper = mapper;
            this.logger = logger;
            this.customerService = customerService;
        }

        [HttpGet()]
        public IActionResult Get()
        {
            return Ok("CustomerController");
        }

        [HttpGet("cart")]
        public async Task<IActionResult> GetCartDetails(string customer_id="1")
        {
           var data =  await customerService.GetCartForCustomer(customer_id);
            return AppResponse.Success(data);
        }

        [HttpGet("order")]
        public async Task<IActionResult> GetOrderDetails(string order_id)
        {
            var data = await customerService.GetOrderDetails(order_id);
            return AppResponse.Success(data);
        }

         

    }
}