using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System;
using AppZeroAPI.Interfaces;
using AppZeroAPI.Entities;
using AppZeroAPI.Models;
using AppZeroAPI.Services;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using AppZeroAPI.Shared;

namespace AppZeroAPI.Controllers
{
    [ApiController]
    [Route("api/Users")]
    public class UserController : BaseController
    {
        private readonly ILogger<UserController> logger;
        private readonly IUnitOfWork unitOfWork;
      

        public UserController(IUnitOfWork unitOfWork, ILogger<UserController> logger)
        {
            this.logger = logger;
            this.unitOfWork = unitOfWork;
        }
        
        [HttpGet] 
        public async Task<IActionResult> GetAll()
        {
            logger.LogInformation("called Product Controller");
            var data = await unitOfWork.Users.GetAllAsync();
            return AppResponse.Success(data);
        }

        [HttpDelete("delete/{id}")] 
        public async Task<IActionResult> deleteProfileId(int? id)
        {
            if (id.HasValue)
            {
                var user = await unitOfWork.Users.DeleteByIdAsync(id.Value);
            }
            return AppResponse.Success();
        }
        // DELETE api/User
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id.HasValue)
            {
                var user = await unitOfWork.Users.DeleteByIdAsync(id.Value);
            }
            
            return AppResponse.Success();
        }
         

        [HttpPost("Add")]
        public async Task<IActionResult> Add([FromBody] RegisterDto  model)
        {
#if (DEBUG)
            if (model.Password != model.ConfirmPassword)
            {
                model.ConfirmPassword = model.Password;
            }
#endif


            var existingUser = await unitOfWork.Users.GetUserByEmailAsync(model.Email);
            if (existingUser != null)
                throw new AppException("An account with the same username already exists");
            var salt = Helper.GenerateSalt();
            var _user = new UserProfile()
            {
                email = model.Email,
                username = Guid.NewGuid().ToString().Replace("-", "")
            };

            (_user.password_hash, _user.password_salt) = Helper.GetPasswordHash(model.Password);
            var encPassword = Helper.Encrypt(model.Password);
            _user.last_modified = DateTime.UtcNow;
            _user.created_on = DateTime.UtcNow;
            _user.password = encPassword;
            if (model.Email.ToLower().IndexOf("admin") > -1)
            {
                _user.role = Role.Admin;
            }
            else if (model.Email.ToLower().IndexOf("user") > -1)
            {
                _user.role = Role.User;
            }
            else
            {
                _user.role = Role.Client;
            }
            _user.language = Langauge.English;
            _user.last_modified = DateTime.UtcNow;
            _user.created_on = DateTime.UtcNow;
            var result = await unitOfWork.Users.AddUserAsync(_user);
            _user.user_id = result;
            return AppResponse.Success(_user); ;
        }

        // PUT api/User
        [HttpPut]
        public async Task<IActionResult> Put([FromBody] UserProfile newUser)
        {
            if (!ModelState.IsValid)
                return BadRequest(new {ErrorMsg = "No se ha proporcionado un usuario valido"});

            var user = GetUser(HttpContext.User).Result;
            newUser.user_id = user.user_id;
            var updatedUser = await this.unitOfWork.Users.UpdateAsync(newUser);

            return Ok(updatedUser);
        }
         
        [HttpGet("GetById/{id}", Name = "GetById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UserProfile>> GetById(int id)
        {
            var data = await unitOfWork.Users.GetByIdAsync(id);
            if (data == null)
            {
                return AppResponse.NotFound("User Not Found");
            } 
            return AppResponse.Success(data);
        }
        [HttpGet("{email}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UserProfile>> GetUserEmailAsync(string email)
        {
            var data = await unitOfWork.Users.GetUserByEmailAsync(email);
            if (data == null)
            {
                return AppResponse.NotFound("User Not Found");
            }
            return AppResponse.Success(data);
        }
        private async Task<UserProfile>  GetUser(ClaimsPrincipal user)
        {
            string textId = user.FindFirst(claim => claim.Type == ClaimTypes.NameIdentifier).Value;

            return await this.unitOfWork.Users.GetByUserIdAsync(textId);
        }
    }
}