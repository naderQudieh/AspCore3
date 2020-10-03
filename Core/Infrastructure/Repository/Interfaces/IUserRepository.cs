using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AppZeroAPI.Entities;
using AppZeroAPI.Models;

namespace AppZeroAPI.Interfaces
{
    public interface IUserRepository : IGenericRepository<UserProfile>
    { 
        Task<UserProfile> GetByUserIdAsync(string userid);
        Task<UserProfile> GetUserByEmailAsync(string email);
        Task<bool> DeleteByIdAsync(int id);
        Task<int> AddUserAsync(UserProfile user);
        Task<bool> ResetPasswordAsync(UserProfile user, string token, string NewPassword);
        Task<string> GeneratePasswordResetTokenAsync(UserProfile user); 
        Task<string> GetUserRoleByIdAsync(string userid);   
        Task<IList<UserTokensData>> GetUserToekns(string userid);
        Task<UserTokenData> GetUserRefreshToken(string refreshToken);
        Task<UserTokenData> GetUserRefreshTokenByTokenId(int refreshTokenId);
   
        Task<int> DeleteRefreshTokenByIdAsync(int refreshTokenId);
        Task<int> UserDeleteTokens(string userid); 
        Task<int> AddRefreshTokenAsync(UserTokenData entity);
        Task BlackListed(int token_id);

    }

}
