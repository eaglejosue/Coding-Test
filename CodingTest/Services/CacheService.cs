namespace Api.Services;

public interface ICacheService
{
    void ClearAll();
    T? Get<T>(string key);
    void Set<T>(string key, T item, TimeSpan? absoluteExpirationRelativeToNow = null);
}

public class CacheService(IMemoryCache cache) : ICacheService
{
    private readonly List<string> _keys = [];

    public void Set<T>(string key, T item, TimeSpan? absoluteExpirationRelativeToNow = null)
    {
        var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(5));

        if (absoluteExpirationRelativeToNow.HasValue)
            cacheEntryOptions.AbsoluteExpirationRelativeToNow = absoluteExpirationRelativeToNow;

        cache.Set(key, item, cacheEntryOptions);

        lock (_keys)
        {
            if (!_keys.Contains(key))
                _keys.Add(key);
        }
    }

    public T? Get<T>(string key) => cache.Get<T>(key);

    public void ClearAll()
    {
        lock (_keys)
        {
            foreach (var key in _keys)
                cache.Remove(key);

            _keys.Clear();
        }
    }
}

