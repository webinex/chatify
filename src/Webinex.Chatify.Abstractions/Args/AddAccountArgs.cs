namespace Webinex.Chatify.Abstractions;

public class AddAccountArgs
{
    public string Id { get; }
    public string WorkspaceId { get; }
    public string? Avatar { get; }
    public string Name { get; }

    public AddAccountArgs(string id, string workspaceId, string? avatar, string name)
    {
        if (id == AccountContext.System.Id)
            throw new ArgumentException("System account cannot be added", nameof(id));
        
        Id = id ?? throw new ArgumentNullException(nameof(id));
        WorkspaceId = workspaceId ?? throw new ArgumentNullException(nameof(workspaceId));
        Avatar = avatar;
        Name = name ?? throw new ArgumentNullException(nameof(name));
    }
}