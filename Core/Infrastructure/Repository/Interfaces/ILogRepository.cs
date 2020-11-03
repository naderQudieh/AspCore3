using AppZeroAPI.Interfaces;
using AppZeroAPI.Entities ;
using System.Threading.Tasks;

namespace AppZeroAPI.Repository
{
    public interface ILogRepository : IGenericRepository<LogData>
    {
        Task AddLog(LogData logData);
    }
}
