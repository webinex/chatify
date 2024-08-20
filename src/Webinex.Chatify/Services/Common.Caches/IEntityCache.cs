namespace Webinex.Chatify.Services.Common.Caches;

internal interface IEntityCache<TValue>
    where TValue : ICloneable
{
    Task<IReadOnlyDictionary<string, TValue>> GetOrCreateAsync(
        IEnumerable<string> ids,
        Func<IEnumerable<string>, Task<IReadOnlyDictionary<string, TValue>>> factory);

    Task InvalidateAsync(IEnumerable<string> ids);
}

internal static class EntityCacheExtensions
{
    public static async Task InvalidateAsync<TValue>(this IEntityCache<TValue> cache, string id)
        where TValue : ICloneable
    {
        cache = cache ?? throw new ArgumentNullException(nameof(cache));
        await cache.InvalidateAsync(new[] { id });
    }
}
