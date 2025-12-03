namespace Webinex.Chatify.Abstractions;

public class AddChatArgs
{
    public string Name { get; }
    public string? WorkspaceId { get; }
    public IReadOnlyCollection<string> Members { get; }
    public MessageBody? Message { get; }
    public AccountContext OnBehalfOf { get; }

    public AddChatArgs(
        string name,
        IReadOnlyCollection<string> members,
        MessageBody? message,
        AccountContext onBehalfOf,
        string? workspaceId = null)
    {
        if (!onBehalfOf.IsSystem() && !members.Contains(onBehalfOf.Id))
            throw new ArgumentException("When create chat not on behalf of system. User might be a member of the chat",
                nameof(onBehalfOf));

        if (workspaceId == AccountContext.System.WorkspaceId)
            throw new ArgumentException("WorkspaceId cannot be system workspace id", nameof(workspaceId));

        if (workspaceId == null && onBehalfOf.IsSystem())
            throw new ArgumentException("When create chat on behalf of system. WorkspaceId must be provided",
                nameof(workspaceId));

        Name = name ?? throw new ArgumentNullException(nameof(name));
        Members = members ?? throw new ArgumentNullException(nameof(members));
        Message = message;
        OnBehalfOf = onBehalfOf ?? throw new ArgumentNullException(nameof(onBehalfOf));
        WorkspaceId = workspaceId;
    }
}
