using AppZeroAPI.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AppZeroAPI.Interfaces
{
    public interface IUserRepository : IGenericRepository<UserProfile>
    {
        Task<UserProfile> GetByUserIdAsync(string userid);
        Task<UserProfile> GetUserByEmailAsync(string email);
         
        Task<int> AddUserAsync(UserProfile user);
        Task<bool> ResetPasswordAsync(UserProfile user, string token, string NewPassword);
        Task<string> GeneratePasswordResetTokenAsync(UserProfile user);
        Task<string> GetUserRoleByIdAsync(string userid);
        Task<IList<UserTokensData>> GetUserToekns(string userid);
        Task<UserTokenData> GetUserRefreshToken(string refreshToken);
        Task<UserTokenData> GetUserRefreshTokenByTokenId(long refreshTokenId);

        Task<int> DeleteRefreshTokenByIdAsync(long refreshTokenId);
        Task<int> UserDeleteTokens(string userid);
        Task<int> AddRefreshTokenAsync(UserTokenData entity);
        Task BlackListed(long token_id);

    }

}
