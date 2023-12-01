namespace Webinex.Chatify.Types;

public class ChatActivity
{
    public Guid ChatId { get; protected init; }
    public string AccountId { get; protected init; } = null!;
    public string LastMessageFromId { get; protected set; } = null!;
    public string LastMessageId { get; protected set; } = null!;

    protected ChatActivity()
    {
    }

    public ChatActivity(Guid chatId, string accountId, string lastMessageFromId, string lastMessageId)
    {
        ChatId = chatId;
        AccountId = accountId;
        LastMessageFromId = lastMessageFromId;
        LastMessageId = lastMessageId;
    }
}