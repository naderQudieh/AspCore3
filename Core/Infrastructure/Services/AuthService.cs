using AppZeroAPI.Entities;
using AppZeroAPI.Interfaces;
using AppZeroAPI.Models;
using AppZeroAPI.Repository;
using AppZeroAPI.Shared;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;

namespace AppZeroAPI.Services
{



    public class AuthService : BaseRepository, IAuthService
    {
        private const string UserIdKey = "id";
        private readonly ILogger<AuthService> logger;
        protected readonly TokenService tokenService;
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper _mapper;

        public AuthService(IConfiguration configuration, ILogger<AuthService> logger,
            IUnitOfWork unitOfWork, TokenService tokenService, IMapper mapper) : base(configuration)
        {
            _mapper = mapper;
            this.unitOfWork = unitOfWork;
            this.logger = logger;
            this.tokenService = tokenService;
        }


        public async Task<ResponseAuthDto> Authenticate(LoginDto model, string ipAddress = "")
        {
            var user = await this.unitOfWork.Users.GetUserByEmailAsync(model.username);
            if (user == null)
                throw new AppException("User not found - Invalid Email");

            if (user.password_hash == Helper.HashPassword(model.password, user.password_salt))
            {
                throw new AppException("Invalid password");
            }

            var userInfo = _mapper.Map<UserInfo>(user);
            TokenDto newAccessToken = tokenService.generateAccessToken(userInfo);
            TokenDto newRefreshToken = tokenService.generateRefreshToken(userInfo);

            var token = new UserTokenData()
            {
                AccessToken = newAccessToken.EncodedToken,
                RefreshToken = newRefreshToken.EncodedToken,
                BlackListed = false,
                AccessTokenExpiresAt = newRefreshToken.TokenModel.ValidTo,
                CreatedAt = newRefreshToken.TokenModel.IssuedAt,
                CreatedByIP = ipAddress,


            };

            token.user_id = user.user_id;
            await this.unitOfWork.Users.AddRefreshTokenAsync(token);
            var tokenInfo = new TokenInfoDto
            {
                AccessToken = newAccessToken.EncodedToken,
                RefreshToken = newRefreshToken.EncodedToken,
                ExpiresAt = newRefreshToken.TokenModel.ValidTo,
                TokenType = JwtBearerDefaults.AuthenticationScheme
            };
            return new ResponseAuthDto
            {
                Token = tokenInfo,
                User = userInfo
            };

        }


        public async Task<ResponseAuthDto> RenewAccessToken(RequestAuthDto request, string ipAddress = "")
        {
            JwtSecurityToken decodedToken = this.tokenService.DecodeToken(request.RefreshToken);
            var user = await this.unitOfWork.Users.GetByIdAsync(int.Parse(decodedToken.Subject));
            if (user == null)
                throw new AppException("Invalid token.");

            var tokenRecord = await this.unitOfWork.Users.GetUserRefreshToken(request.RefreshToken);
            if (tokenRecord == null)
            {
                throw new AppException("Invalid refresh token.");
            }

            //Access token should be verified.
            //because on the token storage it's been save as a paired token (access and refresh token are saved together)
            if (tokenRecord.AccessToken != request.AccessToken)
            {
                throw new AppException("Invalid access token.");
            }

            if (tokenRecord.BlackListed)
            {
                throw new AppException("Token is blacklisted.");
            }

            if (tokenRecord.AccessTokenExpiresAt.Subtract(DateTime.UtcNow).TotalSeconds <= 0)
            {
                throw new AppException("Refresh token is expired.");
            }

            //AutoMapper.Mapper.Map<Destination>(source);
            var userInfo = _mapper.Map<UserInfo>(user);
            TokenDto newAccessToken = tokenService.generateAccessToken(userInfo);
            TokenDto newRefreshToken = tokenService.generateRefreshToken(userInfo);

            await this.unitOfWork.Users.BlackListed(tokenRecord.TokenId);

            var token = new UserTokenData()
            {

                AccessToken = newAccessToken.EncodedToken,
                RefreshToken = newRefreshToken.EncodedToken,
                BlackListed = false,
                AccessTokenExpiresAt = newRefreshToken.TokenModel.ValidTo,
                CreatedAt = newRefreshToken.TokenModel.IssuedAt,
                CreatedByIP = ipAddress
            };
            await this.unitOfWork.Users.AddRefreshTokenAsync(token);
            var tokenInfo = new TokenInfoDto
            {
                AccessToken = newAccessToken.EncodedToken,
                RefreshToken = newRefreshToken.EncodedToken,
                ExpiresAt = newRefreshToken.TokenModel.ValidTo,
                TokenType = JwtBearerDefaults.AuthenticationScheme
            };
            return new ResponseAuthDto
            {
                Token = tokenInfo,
                User = userInfo
            };
        }

        public async Task<int> SignUp(RegisterDto user, string ipAddress = "")
        {
            var existingUser = await unitOfWork.Users.GetUserByEmailAsync(user.Email);
            if (existingUser != null)
                throw new AppException("An account with the same username already exists");
            var salt = Helper.GenerateSalt();
            var _user = new UserProfile()
            {
                email = user.Email,
                username = Guid.NewGuid().ToString().Replace("-", "")
            };

            (_user.password_hash, _user.password_salt) = Helper.GetPasswordHash(user.Password);
            var encPassword = Helper.Encrypt(user.Password); 
            _user.password = encPassword;
            if (user.Email.ToLower().IndexOf("admin") > -1)
            {
                _user.role = Role.Admin;
            }
            else if (user.Email.ToLower().IndexOf("user") > -1)
            {
                _user.role = Role.User;
            }
            else
            {
                _user.role = Role.Client;
            }
            _user.language = Langauge.English; 
            var result = await unitOfWork.Users.AddUserAsync(_user);
            return result;
        }
        public async Task Logout(string userId)
        {
            UserProfile user = await unitOfWork.Users.GetByUserIdAsync(userId);
            if (user != null)
            {
                await unitOfWork.Users.UserDeleteTokens(userId);
            }
        }


        public async Task<bool> RevokeToken(string token)
        {
            JwtSecurityToken decodedToken = this.tokenService.DecodeToken(token);
            int userId = int.Parse(decodedToken.Id);
            var user = await this.unitOfWork.Users.GetByIdAsync(userId);
            if (user == null)
                throw new AppException("Invalid token.");

            int tokenId = int.Parse(decodedToken.Id);
            UserTokenData stoken = await unitOfWork.Users.GetUserRefreshTokenByTokenId(tokenId);
            if (stoken == null)
            {
                throw new AppException("Invalid Token");
            }
            if (!stoken.IsActive)
                return false;
            stoken.BlackListed = true;
            await unitOfWork.Users.BlackListed(tokenId);
            return true;
        }
        public bool ValidateToken(string userId, string token)
        {
            try
            {
                var principal = this.tokenService.getPrincipalFromToken(token);

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
