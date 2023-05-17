using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Infrastructure.Shared.CacheService
{
    public class ResponseCacheService : IResponseCacheService
    {
        private readonly IDistributedCache _distributedCache;
        private readonly IConnectionMultiplexer _connectionMultiplexer;

        public ResponseCacheService(IDistributedCache distributedCache, IConnectionMultiplexer connectionMultiplexer)
        {
            _distributedCache = distributedCache;
            _connectionMultiplexer = connectionMultiplexer;
        }

        public async Task CacheResponseAsync(string cacheKey, object response, TimeSpan timeTimeLive)
        {
            if (response == null)
            {
                return;
            }

            var serializedResponse = JsonConvert.SerializeObject(response);

            await _distributedCache.SetStringAsync(cacheKey, serializedResponse, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = timeTimeLive
            });
        }

        public async Task<string> GetCachedResponseAsync(string cacheKey)
        {
            var cachedResponse = await _distributedCache.GetStringAsync(cacheKey);
            return string.IsNullOrEmpty(cachedResponse) ? null : cachedResponse;
        }

        public void RemoveKey(string[] arrKeyContains)
        {
            IDatabase db = _connectionMultiplexer.GetDatabase();
            EndPoint endPoint = _connectionMultiplexer.GetEndPoints().First();
            RedisKey[] keys = _connectionMultiplexer.GetServer(endPoint).Keys(pattern: "*").ToArray();
            var redisKeys = keys.Where(x => arrKeyContains.Any(y => x.ToString().Contains(y.ToString())));
            db.KeyDelete(redisKeys.ToArray());
        }
    }
}