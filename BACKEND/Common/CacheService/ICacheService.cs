namespace Common.CacheService
{
    public interface ICacheService
    {
        void Create<TItem>(string key, TItem value);
        TItem Get<TItem>(string key);
        void Remove(string key);
    }
}