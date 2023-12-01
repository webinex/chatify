namespace Webinex.Chatify.Types;

public class Account
{
    public string Id { get; protected init; } = null!;
    public string? Avatar { get; protected set; }
    public string Name { get; protected set; } = null!;
    public AccountType Type { get; protected init; }
    public bool Active { get; protected set; }

    protected Account()
    {
    }

    public Account(string id, string name, string? avatar, bool active)
    {
        Id = id ?? throw new ArgumentNullException(nameof(id));
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
}