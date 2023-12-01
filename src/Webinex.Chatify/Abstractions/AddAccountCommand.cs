namespace Webinex.Chatify.Abstractions;

public class AddAccountCommand
{
    public string Id { get; }
    public string? Avatar { get; }
    public string Name { get; }

    public AddAccountCommand(string id, string? avatar, string name)
    {
        Id = id;
        Avatar = avatar;
        Name = name;
    }
}