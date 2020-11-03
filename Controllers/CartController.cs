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
    [Route("api/cart")]
    public class CartController : BaseController
    {
        private readonly ILogger<CartController> logger;
        private readonly IUnitOfWork unitOfWork;

        public CartController(IUnitOfWork unitOfWork, ILogger<CartController> logger)
        {
            this.logger = logger;
            this.unitOfWork = unitOfWork;
        }

        [HttpGet()]
        public IActionResult  Get()
        {
            return Ok("LookupsController");
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CustomerCart>> GetBasketById(string id)
        {
            var cart = await unitOfWork.Carts.GetByIdAsync(id); 
            return Ok(cart ?? new CustomerCart());

        }

        [HttpPost("update")]
        public async Task<ActionResult<CustomerCart>> UpdateBasket(CustomerCart basket)
        {
           // var customerBasket = _mapper.Map<CustomerBasketDto, CustomerCart>(basket); 
            var updatedBasket = await unitOfWork.Carts.UpdateAsync(basket); 
            return Ok(updatedBasket);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteBasketAsync(string id)
        {
            await   unitOfWork.Carts.DeleteByIdAsync(id);
            return Ok();
        }

    }
}