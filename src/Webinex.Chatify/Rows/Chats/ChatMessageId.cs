using Webinex.Chatify.Abstractions;

namespace Webinex.Chatify.Rows.Chats;

internal class ChatMessageId : Equatable
{
    public Guid ChatId { get; protected init; }
    public int Index { get; protected init; }
    public ChatMessageId(Guid chatId, int index)
    {
        ChatId = chatId;
        Index = index;
    }
    
    public static ChatMessageId New(Guid chatId, int index)
    {
        return new ChatMessageId(chatId, index);
    }

    protected ChatMessageId()
    {
    }

    public override string ToString()
    {
        var chatIdPart = ChatId.ToString().ToLowerInvariant();
        var indexPart = Index.ToString().PadLeft(9, '0');
        return $"{chatIdPart}-{indexPart}";
    }

    public static ChatMessageId Parse(string value)
    {
        var chatIdPart = value.Substring(0, 36);
        var indexPart = value.Substring(36 + 1, 9);
        return new ChatMessageId(
            new Guid(chatIdPart),
            int.Parse(indexPart));
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return ChatId;
        yield return Index;
    }
}