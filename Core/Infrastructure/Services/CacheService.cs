using AppZeroAPI.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace AppZeroAPI.Services
{
    internal class CacheService : ICacheService
    {

        private readonly IDistributedCache _cache;

        public CacheService(IDistributedCache cache)
        {
            _cache = cache;
        }

        public async Task<bool> HasKeyAsync(string key)
        {
            return !string.IsNullOrEmpty(await _cache.GetStringAsync(key));
        }

        public async Task<string> GetKeyAsync(string key)
        {
            var cachedValue = await _cache.GetStringAsync(key);
            return string.IsNullOrEmpty(cachedValue) ? null : cachedValue;
        }

        public async Task PutAsync(string key, TimeSpan timeTimeLive, object obj)
        {
            await PutStringAsync(key, timeTimeLive, JsonConvert.SerializeObject(obj));
        }

        public async Task PutStringAsync(string key, TimeSpan timeTimeLive, string value)
        {
            await _cache.SetStringAsync(key, value, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = timeTimeLive
            });
        }

    }
}
