namespace Webinex.Chatify.Abstractions;

public class Account
{
    public string Id { get; }
    public string WorkspaceId { get; }
    public string? Avatar { get; }
    public string Name { get; }
    public AccountType Type { get; }
    public bool Active { get; }

    public Account(string id, string workspaceId, string? avatar, string name, AccountType type, bool active)
    {
        if (!Enum.IsDefined(typeof(AccountType), type))
            throw new ArgumentException($"Unknown enum value {type} for {nameof(AccountType)}");

        Id = id ?? throw new ArgumentNullException(nameof(id));
        WorkspaceId = workspaceId ?? throw new ArgumentNullException(nameof(workspaceId));
        Avatar = avatar;
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Active = active;
        Type = type;
    }
}
