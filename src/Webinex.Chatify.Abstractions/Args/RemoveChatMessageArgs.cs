namespace Webinex.Chatify.Abstractions;

public class RemoveChatMessageArgs
{
    public Guid ChatId { get; }
    public string MessageId { get; }
    public AccountContext OnBehalfOf { get; }

    public RemoveChatMessageArgs(Guid chatId, string messageId, AccountContext onBehalfOf)
    {
        ChatId = chatId;
        MessageId = messageId;
        OnBehalfOf = onBehalfOf ?? throw new ArgumentNullException(nameof(onBehalfOf));
    }
}