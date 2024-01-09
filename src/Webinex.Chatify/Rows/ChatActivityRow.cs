namespace Webinex.Chatify.Rows;

internal class ChatActivityRow
{
    public Guid ChatId { get; protected init; }
    public string AccountId { get; protected init; } = null!;
    public string LastMessageFromId { get; protected set; } = null!;
    public string LastMessageId { get; protected set; } = null!;

    public DeliveryRow? Delivery { get; protected set; } = null!;
    public ChatRow? Chat { get; protected set; } = null!;

    protected ChatActivityRow()
    {
    }

    public ChatActivityRow(Guid chatId, string accountId, string lastMessageFromId, string lastMessageId)
    {
        ChatId = chatId;
        AccountId = accountId;
        LastMessageFromId = lastMessageFromId;
        LastMessageId = lastMessageId;
    }
}