using Core.Models;
using Core.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Core.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IRepository repository;
        private readonly ITokenHandler tokenHandler; 
       
        public AuthenticationService(IRepository repository,  ITokenHandler tokenHandler)
        {
            this.tokenHandler = tokenHandler;
            this.repository = repository; 
        }

        public async Task<TokenResponse>   CreateAccessTokenAsync(string email, string password)
        { 
            var user = await this.repository.Get<User>(x => x.UserName == email);
            
            if (user == null || !Helpers.PasswordHelper.ValidatePassword(password, user.Hash))
            {
                return new TokenResponse(false, "Invalid credentials.", null);
            }

            var token = this.tokenHandler.CreateAccessToken(user);

            return new TokenResponse(true, null, token);
        }

        public async Task<TokenResponse> RefreshTokenAsync(string refreshToken, string userEmail)
        {
            var token = this.tokenHandler.TakeRefreshToken(refreshToken);

            if (token == null)
            {
                return new TokenResponse(false, "Invalid refresh token.", null);
            }

            if (token.IsExpired())
            {
                return new TokenResponse(false, "Expired refresh token.", null);
            }
           
            var user = await this.repository.Get<User>(x => x.UserName == userEmail); 
            if (user == null)
            {
                return new TokenResponse(false, "Invalid refresh token.", null);
            }

            var accessToken = this.tokenHandler.CreateAccessToken(user);
            return new TokenResponse(true, null, accessToken);
        }

        public void RevokeRefreshToken(string refreshToken)
        {
            this.tokenHandler.RevokeRefreshToken(refreshToken);
        }
    }
}