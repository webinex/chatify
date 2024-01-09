namespace Webinex.Chatify.Services.Caches;

internal class ChatMembersCacheEntry : ICloneable
{
    public Guid ChatId { get; protected init; }
    public IEnumerable<string> MemberIds { get; protected init; } = null!;

    public ChatMembersCacheEntry(Guid chatId, IEnumerable<string> memberIds)
    {
        ChatId = chatId;
        MemberIds = memberIds.Distinct().ToArray();
    }

    protected ChatMembersCacheEntry()
    {
    }

    public object Clone()
    {
        return new ChatMembersCacheEntry
        {
            ChatId = ChatId,
            MemberIds = MemberIds.ToArray(),
        };
    }
}
