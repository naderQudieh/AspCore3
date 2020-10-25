using AppZeroAPI.Models;
using AppZeroAPI.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace AppZeroAPI.Services
{



    public class TokenService  
    {
        private JwtSecurityTokenHandler tokenhandler = new JwtSecurityTokenHandler();
        private const string UserIdKey = "id";
        private readonly ILogger<TokenService> logger;
        private readonly IConfiguration configuration;
        private readonly JwtOptions jwtOptions;
        private HttpContext context;
        public TokenService(IConfiguration configuration, IHttpContextAccessor contextAccessor, ILogger<TokenService> logger)
        {
            this.context = contextAccessor.HttpContext;
            this.logger = logger;
            this.configuration = configuration;
            this.jwtOptions = this.configuration.GetSection(nameof(JwtOptions)).Get<JwtOptions>();
        }
        public TokenValidationParameters getTokenValidationParameters()
        {
            return new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidIssuer = jwtOptions.Issuer,
                ValidAudience = jwtOptions.Audience,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SecretKey)),
                RequireExpirationTime = true,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
        }


        public ClaimsPrincipal getSession()
        {
            return this.context?.User;
        }
        public int AccessTokenLifeTimeMints()
        {
            return this.jwtOptions.TokenExpireInMints;
        }
        public TokenDto generateAccessToken(UserInfo user)
        {
            var claims = this.getUserClaims(user).Append(new Claim(JwtRegisteredClaimNames.Typ, "access"));
            JwtSecurityToken token = this.generateToken(user, DateTime.UtcNow.AddMinutes(AccessTokenLifeTimeMints()), claims);
            return new TokenDto()
            {
                EncodedToken = this.tokenhandler.WriteToken(token),
                TokenModel = token,
            };
        }

        public TokenDto generateRefreshToken(UserInfo user)
        {
            var claims = this.getUserClaims(user).Append(new Claim(JwtRegisteredClaimNames.Typ, "refresh"));
            JwtSecurityToken token = this.generateToken(user, DateTime.UtcNow.AddDays(5), claims);
            return new TokenDto()
            {
                EncodedToken = this.tokenhandler.WriteToken(token),
                TokenModel = token,
            };
        }
        private JwtSecurityToken generateToken(UserInfo user, DateTime expires, IEnumerable<Claim> claims)
        {
            var descriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(claims),
                Expires = expires,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SecretKey)), SecurityAlgorithms.HmacSha256),
            };

            return tokenhandler.CreateJwtSecurityToken(descriptor);
        }
        public bool IsRefreshToken(JwtSecurityToken token)
        {
            return token.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Typ)?.Value == "refresh";
        }

        private string generateRefreshTokenX()
        {
            using (var rngCryptoServiceProvider = new RNGCryptoServiceProvider())
            {
                var randomBytes = new byte[64];
                rngCryptoServiceProvider.GetBytes(randomBytes);
                return Convert.ToBase64String(randomBytes);
            }
        }
        public bool isRefreshToken(JwtSecurityToken token)
        {
            return token.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Typ)?.Value == "refresh";
        }



        public string getClaimTypeValue(string accessToken, string keyOrClaimType)
        {
            var jwt = DecodeToken(accessToken);
            if (jwt == null)
            {
                return null;
            }
            var claim = jwt.Claims.FirstOrDefault(x => x.Type == keyOrClaimType);
            if (claim == null)
            {
                return null;
            }
            return claim.Value;
        }

       
        public ClaimsPrincipal getPrincipalFromToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                var tokenValidationParameters = getTokenValidationParameters();
                tokenValidationParameters.ValidateLifetime = false;
                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var validatedToken);
                if (!IsJwtWithValidSecurityAlgorithm(validatedToken))
                {
                    throw new SecurityTokenException("Invalid token");
                    //return null;
                }

                return principal;
            }
            catch
            {
                return null;
            }
        }
        public JwtSecurityToken DecodeToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            if (string.IsNullOrEmpty(token))
                throw new AppException("Invalid token");
            return tokenHandler.ReadJwtToken(token);
        }
        private bool IsJwtWithValidSecurityAlgorithm(SecurityToken validatedToken)
        {
            return (validatedToken is JwtSecurityToken jwtSecurityToken) &&
                   jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                       StringComparison.InvariantCultureIgnoreCase);
        }
        private string getUserId(ClaimsPrincipal principal)
        {
            return principal.Claims.Single(x => x.Type == UserIdKey).Value;
        }
        private string generateAccessToken2(UserInfo user)
        {
            user.fname = string.IsNullOrEmpty(user.fname) ? "UNKOWN" : user.fname;
            var Subject = generateUserClaimsIdentity(user);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = Subject,
                Expires = DateTime.UtcNow.AddMinutes(jwtOptions.TokenExpireInMints),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SecretKey)), SecurityAlgorithms.HmacSha256)
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);
            return tokenString;
        }

        private Claim[] getUserClaims(UserInfo user)
        {
            string userid = user.user_id.ToString() ?? user.email;
            return new[] {
                new Claim("Id", user.user_id.ToString()),
                 new Claim(ClaimTypes.Role, Role.Admin),
               // new Claim(ClaimTypes.Name, user.username), same as UniqueName
                new Claim(JwtRegisteredClaimNames.Sub, userid.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, userid.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };
        }

        private ClaimsIdentity generateUserClaimsIdentity(UserInfo user)
        {
            ClaimsIdentity Subject = new ClaimsIdentity(new Claim[]
                    {
                    new Claim("UserId", user.user_id.ToString()),
                    new Claim("FirstName", user.fname ),
                    new Claim("LastName",user.lname),
                    new Claim("EmailId",user.email ==null?"":user.email),
                    new Claim("UserName",user.user_id.ToString()),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    });
            if (!string.IsNullOrEmpty(user.role))
            {
                Subject.AddClaim(new Claim(ClaimTypes.Role, user.role));
            }
            return Subject;
        }



       

        private string getUserIdFromBearer(string bearer)
        {

            var token = bearer.Replace("Bearer ", "").Replace("bearer ", "");
            ClaimsPrincipal clms = getPrincipalFromToken(token);
            var userid = getUserId(clms);
            return userid;
        }

        public bool ValidateToken(string userId, string token)
        {
            try
            {
                var principal =  getPrincipalFromToken(token);

                var id = principal.Identity.Name;

                if (userId == id) return true;
            }
            catch (Exception ex)
            {
                return false;
            }

            return false;
        }
    }
}