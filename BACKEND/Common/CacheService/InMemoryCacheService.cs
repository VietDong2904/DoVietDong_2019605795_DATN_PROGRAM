using System;
using System.Collections.Concurrent;
using System.Threading;
using Microsoft.Extensions.Caching.Memory;

namespace Common.CacheService
{
    public class InMemoryCacheService : ICacheService
    {
        private readonly IMemoryCache _cache;
        private ConcurrentDictionary<object, SemaphoreSlim> _locks = new ConcurrentDictionary<object, SemaphoreSlim>();

        public InMemoryCacheService(IMemoryCache cache)
        {
            _cache = cache;
        }

        public void Create<TItem>(string key, TItem value)
        {
            if (!_cache.TryGetValue(key, out _))
            {
                //setting up cache options
                var cacheExpiryOptions = new MemoryCacheEntryOptions
                {
                };
                //setting cache entries
                _cache.Set(key, value, cacheExpiryOptions);
            }
        }

        public TItem Get<TItem>(string key)
        {
            var cacheEntry = _cache.Get<TItem>(key);
            return cacheEntry;
        }

        public void Remove(string key)
        {
            _cache.Remove(key);
        }
    }
}
