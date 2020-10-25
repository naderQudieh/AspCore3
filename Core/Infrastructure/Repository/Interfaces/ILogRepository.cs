using AppZeroAPI.Interfaces;
using AppZeroAPI.Models;
using System.Threading.Tasks;

namespace AppZeroAPI.Repository
{
    public interface ILogRepository : IGenericRepository<LogRepository>
    {
        Task AddLog(LogData logData);
    }
}
