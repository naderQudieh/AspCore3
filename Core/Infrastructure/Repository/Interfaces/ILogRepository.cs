using AppZeroAPI.Interfaces;
using AppZeroAPI.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppZeroAPI.Repository
{
    public interface ILogRepository : IGenericRepository<LogRepository>
    {
        Task AddLog(LogData logData);
    }
}
