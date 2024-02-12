using Webinex.Chatify.Abstractions;

namespace Webinex.Chatify.Rows;

internal class MessageId : Equatable
{
    public Guid ChatId { get; protected init; }
    public int Index { get; protected init; }
    public DateTimeOffset CreatedAt { get; protected init; }

    public MessageId(Guid chatId, int index, DateTimeOffset createdAt)
    {
        ChatId = chatId;
        CreatedAt = createdAt.ToUniversalTime();
        Index = index;
    }
    
    public static MessageId New(Guid chatId, int index)
    {
        return new MessageId(chatId, index, DateTimeOffset.UtcNow);
    }

    protected MessageId()
    {
    }

    public override string ToString()
    {
        var chatIdPart = ChatId.ToString().ToUpperInvariant();
        var indexPart = Index.ToString().PadLeft(9, '0');
        var createdAtPart = CreatedAt.UtcTicks.ToString().PadLeft(18, '0');
        return $"{chatIdPart}-{indexPart}-{createdAtPart}";
    }

    public static MessageId Parse(string value)
    {
        var chatIdPart = value.Substring(0, 36);
        var indexPart = value.Substring(36 + 1, 9);
        var createdAtPart = value.Substring(36 + 1 + 9 + 1, 18);
        return new MessageId(
            new Guid(chatIdPart),
            int.Parse(indexPart),
            new DateTimeOffset(long.Parse(createdAtPart), TimeSpan.Zero));
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return ChatId;
        yield return Index;
        yield return CreatedAt;
    }
}