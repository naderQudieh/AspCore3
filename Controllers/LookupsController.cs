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
    [Route("api/lookups")]
    public class LookupsController : BaseController
    {
        private readonly ILogger<LookupsController> logger;
        private readonly IUnitOfWork unitOfWork;

        public LookupsController(IUnitOfWork unitOfWork, ILogger<LookupsController> logger)
        {
            this.logger = logger;
            this.unitOfWork = unitOfWork;
        }


        [HttpGet("/countries")]
        public async Task<IActionResult> GetCountries()
        {
            
            var data = await unitOfWork.Lookups.GetCountries();
            return AppResponse.Success(data);
        }

        [HttpGet("/states")]
        public async Task<IActionResult> GetStates()
        {
        
            var data = await unitOfWork.Lookups.GetStates();
            return AppResponse.Success(data);
        }

        [HttpGet("/languages")]
        public async Task<IActionResult> GetLanguages()
        { 
            var data = await unitOfWork.Lookups.GetLanguages();
            return AppResponse.Success(data);
        }

    }
}