using AppZeroAPI.Interfaces;
using AppZeroAPI.Middleware;
using AppZeroAPI.Models;
using AppZeroAPI.Shared;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;



namespace AppZeroAPI.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : BaseController
    {

        private readonly IAuthService authService;
        private readonly ILogger<AuthController> logger;
        private readonly IMapper _mapper;
        public AuthController(ILogger<AuthController> logger, IMapper mapper, IAuthService authService)
        {
            _mapper = mapper;
            this.logger = logger;
            this.authService = authService;
        }
       
        [HttpGet()]
        public IActionResult Get()
        {
            return Ok("AuthController");
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("Authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] LoginDto model)
        {
            if (string.IsNullOrEmpty(model.username)
                || string.IsNullOrEmpty(model.password))
            {
                return AppResponse.BadRequest("All fields are required");
            }


            ModelValidator.Validate(model);
            string ipaddress = Helper.getIPAddress(this.Request);
            var authResponse = await authService.Authenticate(model, ipaddress);
            if (authResponse == null || authResponse.Token == null)
                return AppResponse.Unauthorized("Invalid Token");

            if (string.IsNullOrEmpty(authResponse.Token.AccessToken) || string.IsNullOrEmpty(authResponse.Token.RefreshToken))
                return AppResponse.Unauthorized("Invalid Token");

            setTokenCookie(authResponse.Token.RefreshToken);
            return AppResponse.Success(authResponse);

        }


        [HttpPost]
        [Route("refresh-token")]
        public async Task<IActionResult> RenewAccessToken([FromBody] RequestAuthDto request)
        {
            ModelValidator.Validate(request);
            var refreshToken = Request.Cookies["refreshToken"];
            string ipaddress = Helper.getIPAddress(this.Request);
            var authResponse = await authService.RenewAccessToken(request, ipaddress);
            if (authResponse == null)
                return AppResponse.Unauthorized("Invalid Token");

            if (string.IsNullOrEmpty(authResponse.Token.AccessToken) || string.IsNullOrEmpty(authResponse.Token.RefreshToken))
                return AppResponse.Unauthorized("Invalid Token");
            setTokenCookie(authResponse.Token.RefreshToken);
            return AppResponse.Success(authResponse);
        }

        [HttpPost("signup")]
        public async Task<IActionResult> Register(string Name, string Email, string Password, string ConfirmPassword)
        {

            if (string.IsNullOrEmpty(Name)
                || string.IsNullOrEmpty(Email)
                || string.IsNullOrEmpty(Password)
                || string.IsNullOrEmpty(ConfirmPassword))
            {
                return BadRequest(AppResponse.BadRequest("All fields are required"));
            }
            var model = new RegisterDto()
            {
                Name = Name,
                Email = Email,
                Password = Password,
                ConfirmPassword = ConfirmPassword,
            };
            await authService.SignUp(model, Request.Headers["origin"]);
            return AppResponse.Success("Registration successful, please check your email for verification instructions");
        }

        [HttpPost("revoke-token")]
        public async Task<IActionResult> RevokeToken([FromBody] string token)
        {
            // accept token from request body or cookie
            var _token = token ?? Request.Cookies["refreshToken"];

            if (string.IsNullOrEmpty(token))
                return BadRequest(new { message = "Token is required" });

            bool response = await authService.RevokeToken(_token);
            if (!response)
                return NotFound(new { message = "Token not found" });

            return Ok(new { message = "Token revoked" });
        }

        [HttpGet("ValidateToken")]
        public IActionResult ValidateToken(string Userid, string token)
        {
            try
            {
                var isValid = this.authService.ValidateToken(Userid, token);

                return Ok(isValid);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return NotFound(ex.Message);
            }

        }
        [NonAction]
        public ActionResult Get(int start, int count)
        {
            throw new NotImplementedException();
        }

        //[Authorize]
        //[HttpPost("tokens/{id}")]
        //public IActionResult GetRefreshTokens(string id)
        //{
        //    var user = this.authService.GetById(id);
        //    return Ok(user.RefreshTokens);
        //}

        private void setTokenCookie(string token)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddDays(7)
            };
            Response.Cookies.Append("refreshToken", token, cookieOptions);
        }
    }
}