namespace Webinex.Chatify.Abstractions;

public class UpdateAccountDataArgs
{
    public string Id { get; }
    public Optional<string> Name { get; }
    public Optional<string> Avatar { get; }
    public Optional<bool> Active { get; }
    public Optional<AutoReply> AutoReply { get; }

    public UpdateAccountDataArgs(string id, Optional<string> name, Optional<string> avatar, Optional<bool> active, Optional<AutoReply> autoReply)
    {
        if (id == AccountContext.System.Id)
            throw new ArgumentException("System account cannot be updated", nameof(id));

        Id = id ?? throw new ArgumentNullException(nameof(id));
        Name = name;
        Avatar = avatar;
        Active = active;
        AutoReply = autoReply;
    }
}