namespace Webinex.Chatify.Services.Caches.Common;

internal class EntityMemoryCacheSettings<TValue>
{
    public EntityMemoryCacheSettings(Func<TValue, string> keySelector, TimeSpan expiration, string entityKey)
    {
        KeySelector = keySelector;
        Expiration = expiration;
        EntityKey = entityKey;
    }

    public Func<TValue, string> KeySelector { get; }
    public TimeSpan Expiration { get; }
    public string EntityKey { get; }
}
