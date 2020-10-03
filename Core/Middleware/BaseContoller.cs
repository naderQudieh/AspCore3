using AppZeroAPI.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppZeroAPI.Controllers
{
    public abstract class BaseController : ControllerBase
    {
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
    }
}
