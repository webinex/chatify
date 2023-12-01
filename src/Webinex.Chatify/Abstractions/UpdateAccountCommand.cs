namespace Webinex.Chatify.Abstractions;

public class UpdateAccountCommand
{
    public string Id { get; }
    public string Name { get; }
    public string? Avatar { get; }
    public bool Active { get; }

    public UpdateAccountCommand(string id, string name, string? avatar, bool active)
    {
        Id = id;
        Name = name;
        Avatar = avatar;
        Active = active;
    }
}