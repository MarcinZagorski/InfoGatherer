using InfoGatherer.api.Services.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace InfoGatherer.api.Services
{
    public class MemoryCacheService : IMemoryCacheService
    {
        private readonly IMemoryCache _memoryCache;

        public MemoryCacheService(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public T Get<T>(string key)
        {
            _memoryCache.TryGetValue(key, out T value);
            return value;
        }

        public void Set<T>(string key, T value, TimeSpan? expirationTime = null)
        {
            var options = new MemoryCacheEntryOptions();
            if (expirationTime.HasValue)
            {
                options.SetAbsoluteExpiration(expirationTime.Value);
            }

            _memoryCache.Set(key, value, options);
        }
    }
}
