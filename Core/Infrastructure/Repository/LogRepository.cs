using AppZeroAPI.Models;
using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace AppZeroAPI.Repository
{
    public class LogRepository : BaseRepository
    {
        private readonly ILogger<LogRepository> logger;
        private readonly ILogRepository _logData;
        public LogRepository(IConfiguration configuration, ILogRepository logData, ILogger<LogRepository> logger) : base(configuration)
        {
            this.logger = logger;
            this._logData = logData;
        }
        public async Task<int> AddLog(LogData entity)
        {
            entity.MessageOn = DateTime.Now;
            var sql = @"Insert into LogData (Category,Message,User,UserId,MessageOn) " +
                "VALUES (@Category,@Message,@User,@UserId,@MessageOn)";
            using (var connection = this.GetOpenConnection())
            {
                var result = await connection.ExecuteAsync(sql, entity);
                return result;
            }
        }

    }
}
