namespace Webinex.Chatify.Abstractions;

public class SendChatMessageArgs
{
    public Guid ChatId { get; }
    public MessageBody Body { get; }
    public AccountContext OnBehalfOf { get; }
    public string? RequestId { get; }

    public SendChatMessageArgs(Guid chatId, MessageBody body, AccountContext onBehalfOf, string? requestId)
    {
        ChatId = chatId;
        Body = body ?? throw new ArgumentNullException(nameof(body));
        OnBehalfOf = onBehalfOf ?? throw new ArgumentNullException(nameof(onBehalfOf));
        RequestId = requestId;
    }
}