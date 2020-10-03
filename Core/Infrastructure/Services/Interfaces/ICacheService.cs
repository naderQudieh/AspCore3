using System;
using System.Threading.Tasks;

namespace AppZeroAPI.Interfaces
{
    public interface ICacheService
    {

        Task<bool> HasKeyAsync(string key);

        Task<string> GetKeyAsync(string key);

        Task PutAsync(string key, TimeSpan timeTimeLive, object obj);

        Task PutStringAsync(string key, TimeSpan timeTimeLive, string value);

    }
}
