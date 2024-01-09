using Webinex.Chatify.Abstractions;

namespace Webinex.Chatify.Rows;

internal class AccountRow : ICloneable
{
    public string Id { get; protected init; } = null!;
    public string WorkspaceId { get; protected init; } = null!;
    public string? Avatar { get; protected set; }
    public string Name { get; protected set; } = null!;
    public AccountType Type { get; protected init; }
    public bool Active { get; protected set; }

    protected AccountRow()
    {
    }

    public AccountRow(string id, string workspaceId, string name, string? avatar, bool active)
    {
        Id = id ?? throw new ArgumentNullException(nameof(id));
        WorkspaceId = workspaceId ?? throw new ArgumentNullException(nameof(workspaceId));
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Avatar = avatar;
        Active = active;
        Type = AccountType.Default;
    }

    public void UpdateName(string name)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
    }

    public void UpdateAvatar(string? avatar)
    {
        Avatar = avatar;
    }

    public void UpdateActive(bool value)
    {
        Active = value;
    }

    public object Clone()
    {
        return new AccountRow
        {
            Active = Active,
            Avatar = Avatar,
            Id = Id,
            WorkspaceId = WorkspaceId,
            Name = Name,
            Type = Type,
        };
    }

    public Account ToAbstraction()
    {
        return new Account(Id, WorkspaceId, Avatar, Name, Type, Active);
    }
}