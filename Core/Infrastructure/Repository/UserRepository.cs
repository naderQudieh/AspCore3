using AppZeroAPI.Entities;
using AppZeroAPI.Interfaces;
using Dapper;
using Dapper.Contrib.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AppZeroAPI.Repository
{
    public class UserRepository : BaseRepository, IUserRepository
    {
        private List<UserProfile> _users = new List<UserProfile>
        {
            new UserProfile { user_id = 1, username = "1",  fname = "Test", email  = "test@test.com", password = "test" , role="Admin" },
            new UserProfile { user_id = 2, username = "2",  fname = "Test2", email  = "test2@test2.com", password = "test2" , role="User" }
        };

        private readonly ILogger<UserRepository> logger;
        public UserRepository(IConfiguration configuration, ILogger<UserRepository> logger) : base(configuration)
        {
            this.logger = logger;
            //_logger= logger.CreateLogger< UserRepository>();
        }
        public async Task<int> AddUserAsync(UserProfile entity)
        {
            entity.created_on = DateTime.UtcNow;
            entity.last_modified = DateTime.UtcNow;
            //last_insert_rowid
            var sql = @"Insert into user_profiles(username,fname,lname,email,phone,password,password_hash,password_salt,role,language,profile_picture,last_modified,created_on)
                VALUES (@username,@fname,@lname,@email,@phone,@password,@password_hash,@password_salt,@role, @language,@profile_picture,@last_modified,@created_on)";
            using (var connection = this.GetOpenConnection())
            {
                return await connection.ExecuteAsync(sql, entity);
            }

        }
        public async Task<int> AddAsync(UserProfile entity)
        {
            entity.created_on = DateTime.UtcNow;
            entity.last_modified = DateTime.UtcNow;
            //last_insert_rowid
            var sql = @"Insert into user_profiles(username,fname,lname,email,phone,password,password_hash, password_salt,role,language,profile_picture,created_on)
                VALUES (@username,@fname,@lname,@email,@phone,@password,@password_hash,@password_salt,@role, @language,@profile_picture,@created_on)";
            using (var connection = this.GetOpenConnection())
            {
                return await connection.ExecuteAsync(sql, entity);
            }

        }
        public async Task<bool> UpdateAsync(UserProfile entity)
        {
            // var user = dto.MapTo<TUser>();
            // await userRepository.Edit(user, Session);
            // return user.MapTo<UserDTO>();
            var sql = @"UPDATE user_profiles SET fname = @fname, lname = @lname, phone = @phone, language = @language, username = @username ,last_modified=@last_modified  WHERE user_id = @user_id";
            using (var connection = this.GetOpenConnection())
            {
                var rows = await connection.ExecuteAsync(sql, entity);
                return rows != 0;
            }
        }

        public async Task<bool> UpdateUserSettingsAsync(UserProfile user, CancellationToken cancellationToken)
        {
            using (var connection = this.GetOpenConnection())
            {
                var sql = @"
					update
						user
					set
						fname = @fname,
						lname = @lname,
						language = @Language,
						profile_picture = @ProfilePicture,
						last_modified = @LastModified
					where
						id = @Id
					";
                var rows = await connection.ExecuteAsync(
                    new CommandDefinition(sql,
                        new
                        {
                            user.fname,
                            user.lname,
                            user.language,
                            user.profile_picture,
                            user.last_modified,
                            user.user_id
                        }, cancellationToken: cancellationToken));
                return rows != 0;
                 
            }
        }

        public async Task<UserProfile> GetByIdAsync(long user_id)
        {

            var sql = "SELECT * FROM user_profiles WHERE user_id = @user_id";
            using (var connection = this.GetOpenConnection())
            {

                var result = await connection.QuerySingleOrDefaultAsync<UserProfile>(sql, new { user_id = user_id });
                return result;
            }
        }

        public async Task<UserProfile> GetByUserIdAsync(string userid)
        {
            var sql = "SELECT * FROM user_profiles WHERE userid = @userid";
            using (var connection = this.GetOpenConnection())
            {

                var result = await connection.QuerySingleOrDefaultAsync<UserProfile>(sql, new { userid = userid });
                return result;
            }
        }
        public async Task<UserProfile> GetUserByEmailAsync(string email)
        {
            var sql = "SELECT * FROM user_profiles WHERE email = @email";
            using (var connection = this.GetOpenConnection())
            {

                var result = await connection.QuerySingleOrDefaultAsync<UserProfile>(sql, new { email = email });
                return result;
            }
        }

        public async Task<IEnumerable<UserProfile>> GetAllAsync()
        {
            var sql = "SELECT * FROM user_profiles";
            using (var connection = this.GetOpenConnection())
            {

                var result = await connection.QueryAsync<UserProfile>(sql);
                return result.ToList();
            }
        }
        public async Task<string> GetUserRoleByIdAsync(string id)
        {
            var sql = "SELECT * FROM user_profiles WHERE Id = @Id";
            using (var connection = this.GetOpenConnection())
            {

                var result = await connection.QuerySingleOrDefaultAsync<UserProfile>(sql, new { Id = id });
                return result.role;
            }
        }


        public async Task<string> GeneratePasswordResetTokenAsync(UserProfile user)
        {
            var sql = "SELECT * FROM user_profiles WHERE RefreshToken = @refreshToken";
            using (var connection = this.GetOpenConnection())
            {

                var result = await connection.QuerySingleOrDefaultAsync<UserProfile>(sql);
                return "";
            }
        }
        public async Task<bool> ResetPasswordAsync(UserProfile user, string token, string NewPassword)
        {
            var sql = "SELECT * FROM user_profiles WHERE RefreshToken = @refreshToken";
            using (var connection = this.GetOpenConnection())
            {

                var result = await connection.QuerySingleOrDefaultAsync<UserProfile>(sql);
                return true;
            }
        }
        public async Task<UserProfile> GetUserByToken(string token)
        {
            var sql = "SELECT * FROM user_profiles WHERE token = @token";
            using (var connection = this.GetOpenConnection())
            {

                var result = await connection.QuerySingleOrDefaultAsync<UserProfile>(sql, new { email = token });
                return result;
            }
        }

        private UserProfile ValidatePassword(string email, string password)
        {
            UserProfile user = GetUserByEmailAsync(email).Result;

            //var hash = VerifypasswordHash(password, user.password_hash , user.password_salt );
            // if (hash) throw new InvalidOperationException("Unable to authenticate user.");

            return user;
        }


        private async Task<bool> UserExist(string userName)
        {
            UserProfile user = await GetUserByEmailAsync(userName);
            if (user != null)
                return true;
            else
                return false;
        }


        public async Task<bool> DeleteByIdAsync(long id)
        {
            //var sql = "delete from user_profiles WHERE user_id = @userid";
            //using (var connection = this.GetOpenConnection())
            //{

            //    var result = await connection.ExecuteAsync(sql, new { userid = id });
            //    return true;
            //}
            using (var connection = this.GetOpenConnection())
            {
                var result = await connection.DeleteAsync(new UserProfile { user_id = id });
                return result;
            }
        }


        public async Task<int> AddRefreshTokenAsync(UserTokenData entity)
        {
            entity.CreatedAt = DateTime.UtcNow;
            var sql = @"Insert into user_refresh_tokens ([user_id],[access_token],[refresh_token],[expires_at],black_listed,created_by_ip,[created_at] )
                        VALUES (@user_id,@AccessToken, @RefreshToken,@ExpiresAt,@BlackListed,@CreatedByIP, @CreatedAt)";
            using (var connection = this.GetOpenConnection())
            {
                var result = await connection.ExecuteAsync(sql, entity);
                return result;
            }
        }
        public async Task<UserTokenData> GetUserRefreshTokenByTokenId(long refreshTokenId)
        {
            var args = new { @token_id = refreshTokenId };

            const string sql = "SELECT * FROM user_refresh_tokens WHERE token_id = @token_id";

            using (var connection = this.GetOpenConnection())
            {

                return await connection.QuerySingleOrDefaultAsync<UserTokenData>(sql, args);
            }
        }
        public async Task<UserTokenData> GetUserRefreshToken(string refreshToken)
        {
            var args = new { refresh_token = refreshToken };

            const string sql = "SELECT * FROM user_refresh_tokens WHERE refresh_token = @refresh_token";

            using (var connection = this.GetOpenConnection())
            {

                return await connection.QuerySingleOrDefaultAsync<UserTokenData>(sql, args);
            }
        }

        public async Task<IList<UserTokensData>> GetUserToekns(string userid)
        {
            var args = new { user_id = userid };
            var sql = @"select *   from user_refresh_tokens   WHERE  u.user_id = @user_id";
            using (var connection = this.GetOpenConnection())
            {

                var products = await connection.QueryAsync<UserTokensData>(sql, args);

                return products.ToList();
            }
        }


        public async Task<int> DeleteRefreshTokenByIdAsync(long refreshTokenId)
        {
            var args = new { @token_id = refreshTokenId };
            const string sql = "DELETE FROM user_refresh_tokens WHERE token_id = @token_id";

            using (var connection = this.GetOpenConnection())
            {
                var result = await connection.ExecuteAsync(sql, args);
                return result;
            }
        }
        public async Task<int> UserDeleteTokens(string userId)
        {
            var args = new { userId = userId };
            const string sql = "DELETE FROM user_refresh_tokens WHERE userId = @userId";

            using (var connection = this.GetOpenConnection())
            {
                var result = await connection.ExecuteAsync(sql, args);
                return result;
            }
        }
        public async Task BlackListed(long token_id)
        {
            var args = new { @token_id = token_id };

            const string sql = "update  user_refresh_tokens set black_listed = true WHERE token_id = @token_id";

            using (var connection = this.GetOpenConnection())
            {
                var result = await connection.ExecuteAsync(sql, args);
            }
        }


    }
}
