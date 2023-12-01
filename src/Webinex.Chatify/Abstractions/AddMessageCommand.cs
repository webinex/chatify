namespace Webinex.Chatify.Abstractions;

public class AddMessageCommand
{
    public Guid ChatId { get; }
    public MessageContent Content { get; }
    public AccountContext OnBehalfOf { get; }
    public string? RequestId { get; }

    public AddMessageCommand(Guid chatId, MessageContent content, AccountContext onBehalfOf, string? requestId)
    {
        ChatId = chatId;
        Content = content;
        OnBehalfOf = onBehalfOf;
        RequestId = requestId;
    }
}