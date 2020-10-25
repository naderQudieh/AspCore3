using AppZeroAPI.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace AppZeroAPI.Services
{

    public interface ITokenService
    {
        TokenDto generateAccessToken(UserInfo user);

        TokenDto generateRefreshToken(UserInfo user);

        ClaimsPrincipal getPrincipalFromToken(string token);

        bool IsRefreshToken(JwtSecurityToken token);
        int AccessTokenLifeTimeMints();
        ClaimsPrincipal getSession();
        JwtSecurityToken decodeToken(string accessToken);
        TokenValidationParameters getTokenValidationParameters();
    }
}
