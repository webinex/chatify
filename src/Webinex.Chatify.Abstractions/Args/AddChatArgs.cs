namespace Webinex.Chatify.Abstractions;

public class AddChatArgs
{
    public string Name { get; }
    public IReadOnlyCollection<string> Members { get; }
    public MessageBody? Message { get; }
    public AccountContext OnBehalfOf { get; }
    public string? RequestId { get; }

    public AddChatArgs(
        string name,
        IReadOnlyCollection<string> members,
        MessageBody? message,
        AccountContext onBehalfOf,
        string? requestId)
    {
        if (!onBehalfOf.IsSystem() && !members.Contains(onBehalfOf.Id))
            throw new ArgumentException("When create chat not on behalf of system. User might be a member of the chat",
                nameof(onBehalfOf));

        Name = name ?? throw new ArgumentNullException(nameof(name));
        Members = members ?? throw new ArgumentNullException(nameof(members));
        Message = message;
        OnBehalfOf = onBehalfOf ?? throw new ArgumentNullException(nameof(onBehalfOf));
        RequestId = requestId;
    }
}
