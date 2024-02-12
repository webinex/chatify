namespace Webinex.Chatify.Services.Caches;

internal class ChatMembersCacheEntry : ICloneable
{
    public Guid ChatId { get; protected init; }
    public IEnumerable<string> Active { get; protected init; } = null!;

    public ChatMembersCacheEntry(Guid chatId, IEnumerable<string> active)
    {
        ChatId = chatId;
        Active = active.Distinct().ToArray();
    }

    protected ChatMembersCacheEntry()
    {
    }

    public object Clone()
    {
        return new ChatMembersCacheEntry
        {
            ChatId = ChatId,
            Active = Active.ToArray(),
        };
    }
}
