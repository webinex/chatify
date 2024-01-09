namespace Webinex.Chatify.Abstractions;

public class ChatMessageId : Equatable
{
    public Guid ChatId { get; }
    public string MessageId { get; }

    public ChatMessageId(Guid chatId, string messageId)
    {
        ChatId = chatId;
        MessageId = messageId ?? throw new ArgumentNullException(nameof(messageId));
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return ChatId;
        yield return MessageId;
    }
}
