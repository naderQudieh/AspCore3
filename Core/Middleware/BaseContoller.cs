using AppZeroAPI.Entities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace AppZeroAPI.Controllers
{
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public  class BaseController : ControllerBase
    {
        
        public BaseController( )  {  
        }

      
        protected async Task<UserProfile> CurrentUser()
        { 
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            UserProfile user = null;// await _userRepository.FirstOrDefaultAsync(x => x.Authentication.AccessToken == accessToken)
                                    //?? throw new UnauthorizedAccessException();

            return user;
        }

        protected Detect GetUserHardwareInfo()
        {
            var userAgent = HttpContext.Request.Headers["User-Agent"];
            var result = DeviceDetectorNET.DeviceDetector.GetInfoFromUserAgent(userAgent);

            var detect = new Detect
            {
                DeviceType = result.Match.DeviceType,
                Browser = result.Match.BrowserFamily,
                Os = result.Match.OsFamily,
                UserIp = HttpContext.Connection.RemoteIpAddress.ToString()
            };
            return detect;
        }
        protected string CurrentUserId
        {
            get
            {
                var userId = Request.HttpContext.Items["user"] as long?;
                if (userId == null || !userId.HasValue)
                    throw new Exception("Unable to get user ID from request");
                return userId.Value.ToString();
            }
        }
        protected string GetAuthorizationHeaderValue()
        {
            var authHeaderValue = Request.Headers["Authorization"].FirstOrDefault();
            if (authHeaderValue == null)
            {
                return null;
            }
            var accessToken = authHeaderValue.Replace("Bearer ", "");
            return accessToken;
        }
        protected int GetUserIDFromToken()
        {
            return 1;
            // return tokenHandler.GetSubValue(GetAuthorizationHeaderValue());
        }

        protected List<Claim> GetJwtClaims(string jwt)
        {
            var claims = new List<Claim>();
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(jwt);
            var token = handler.ReadToken(jwt) as JwtSecurityToken; 
            return ((List<Claim>)token?.Claims);
        }
        protected int GetUserIdFromToken()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            var claims = identity.Claims;
            var nameIdentifier = claims.SingleOrDefault(x => x.Type == ClaimTypes.NameIdentifier);
            if (nameIdentifier == null)
            {
                return -1;
            }
            var id = nameIdentifier.Value;
            return Convert.ToInt32(id);
        }
    }
}
