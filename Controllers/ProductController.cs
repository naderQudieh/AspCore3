using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;


using AppZeroAPI.Interfaces;
using AppZeroAPI.Models;
using AppZeroAPI.Entities;
using Microsoft.AspNetCore.Authorization;
using AppZeroAPI.Shared;

namespace AppZeroAPI.Controllers
{
    //[AuthorizeUser]
    [ApiController]
    [Route("api/Products")]
    public class ProductController : BaseController
    {
        private readonly ILogger<ProductController>  logger;
        private readonly IUnitOfWork unitOfWork;

        public ProductController(IUnitOfWork  unitOfWork, ILogger<ProductController>  logger)
        {
            this.logger = logger;
            this.unitOfWork = unitOfWork;
        }

     
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            logger.LogInformation("called ProductController"); 
            var data = await unitOfWork.Products.GetAllAsync();
            return AppResponse.Success(data);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Product>> GetById(int id)
        {
            var data = await unitOfWork.Products.GetByIdAsync(id);
            if (data==null)
            {
                return NotFound("Product Not Found") ;
            }

            return AppResponse.Success(data);
        }
 

        [HttpPost]
        public async Task<IActionResult> Add(Product product)
        {
            var data = await unitOfWork.Products.AddAsync(product);
            return AppResponse.Success(data);
        }
        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            await Task.Delay(3000);
            //await unitOfWork.Products.DeleteByIdAsync(id);
            return AppResponse.Success();
        }
        [HttpPut]
        public async Task<IActionResult> Update(Product product)
        {
            var data = await unitOfWork.Products.UpdateAsync(product);
            return AppResponse.Success(data);
        }
    }
}