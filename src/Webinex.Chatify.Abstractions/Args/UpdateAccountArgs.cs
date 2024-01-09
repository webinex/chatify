namespace Webinex.Chatify.Abstractions;

public class UpdateAccountArgs
{
    public string Id { get; }
    public string Name { get; }
    public string? Avatar { get; }
    public bool Active { get; }

    public UpdateAccountArgs(string id, string name, string? avatar, bool active)
    {
        if (id == AccountContext.System.Id)
            throw new ArgumentException("System account cannot be updated", nameof(id));
        
        Id = id ?? throw new ArgumentNullException(nameof(id));
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Avatar = avatar;
        Active = active;
    }
}