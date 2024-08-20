using Microsoft.Extensions.Caching.Memory;

namespace Webinex.Chatify.Services.Common.Caches;

internal class EntityMemoryCache<TValue> : IEntityCache<TValue>
    where TValue : ICloneable
{
    private readonly IMemoryCache _memoryCache;
    private readonly EntityMemoryCacheSettings<TValue> _settings;

    public EntityMemoryCache(
        EntityMemoryCacheSettings<TValue> settings,
        IMemoryCache memoryCache)
    {
        _settings = settings;
        _memoryCache = memoryCache;
    }

    public async Task<IReadOnlyDictionary<string, TValue>> GetOrCreateAsync(
        IEnumerable<string> ids,
        Func<IEnumerable<string>, Task<IReadOnlyDictionary<string, TValue>>> factory)
    {
        ids = ids.Distinct().ToArray();
        if (!ids.Any()) return new Dictionary<string, TValue>();

        var keys = ids.Select(Key).ToArray();
        var found = keys.Select(_memoryCache.Get<TValue>).Where(x => x != null).Cast<TValue>().ToArray();
        var notFoundId = ids.Except(found.Select(_settings.KeySelector)).ToArray();

        if (!notFoundId.Any())
            return found.ToDictionary(_settings.KeySelector);

        // Note: in case of concurrency, may be some entities would be loaded twice
        var notFoundAccountById = await factory(notFoundId);

        foreach (var (id, notFoundAccount) in notFoundAccountById)
        {
            _memoryCache.Set(Key(id), (TValue)notFoundAccount.Clone(), _settings.Expiration);
        }

        return found.Concat(notFoundAccountById.Values).ToDictionary(_settings.KeySelector);
    }

    public Task InvalidateAsync(IEnumerable<string> ids)
    {
        foreach (var id in ids)
        {
            _memoryCache.Remove(Key(id));
        }

        return Task.CompletedTask;
    }

    private string Key(string key)
    {
        return $"chatify::{_settings.EntityKey}::{key}";
    }
}