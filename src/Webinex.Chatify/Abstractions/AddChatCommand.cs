namespace Webinex.Chatify.Abstractions;

public class AddChatCommand
{
    public string Name { get; }
    public IReadOnlyCollection<string> Members { get; }
    public MessageContent? Message { get; }
    public AccountContext OnBehalfOf { get; }
    public string? RequestId { get; }

    public AddChatCommand(
        string name,
        IReadOnlyCollection<string> members,
        MessageContent? message,
        AccountContext onBehalfOf,
        string? requestId)
    {
        Name = name;
        Members = members;
        Message = message;
        OnBehalfOf = onBehalfOf;
        RequestId = requestId;
    }
}