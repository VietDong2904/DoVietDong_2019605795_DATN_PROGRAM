using System;
using System.Threading.Tasks;

namespace Infrastructure.Shared.CacheService
{
    public interface IResponseCacheService
    {
        Task CacheResponseAsync(string cacheKey, object response, TimeSpan timeTimeLive);

        Task<string> GetCachedResponseAsync(string cacheKey);

        void RemoveKey(string[] arrKeyContains);
    }
}