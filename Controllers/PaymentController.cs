using AppZeroAPI.Entities;
using AppZeroAPI.Interfaces;
using AppZeroAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace AppZeroAPI.Controllers
{
    //[AuthorizeUser]
    [ApiController]
    [Route("api/payments")]
    public class PaymentController : BaseController
    {
        private readonly ILogger<PaymentController> logger;
        private readonly IUnitOfWork unitOfWork;

        public PaymentController(IUnitOfWork unitOfWork, ILogger<PaymentController> logger)
        {
            logger.LogInformation("called PaymentController");
            this.logger = logger;
            this.unitOfWork = unitOfWork;
        }

        [HttpGet()]
        public IActionResult Get()
        {
            return Ok("PaymentController");
        }
        [HttpGet("/pmt/{customer_id}/{payment_id}")]
        public async Task<IActionResult> GetCustomerPayment([FromQuery] long customer_id,string payment_id)
        { 
            var data = await unitOfWork.Payments.GetCustomerPayment(customer_id, payment_id);
            return AppResponse.Success(data);
        }

        [HttpGet("/pmt/{customer_id}")]
        public async Task<IActionResult> GetCustomerPayments([FromQuery] long customer_id)
        { 
            var data = await unitOfWork.Payments.GetCustomerPayments(customer_id);
            return AppResponse.Success(data);
        }


    }
}