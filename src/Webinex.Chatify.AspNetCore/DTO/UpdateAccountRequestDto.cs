using Webinex.Chatify.Abstractions;

namespace Webinex.Chatify.AspNetCore;

public class UpdateAccountRequestDto
{
    public UpdateAccountRequestDto(string id, Optional<string>? name, Optional<string>? avatar, Optional<bool>? active, Optional<AutoReply>? autoReply)
    {
        Id = id;
        Name = name ?? Optional.NoValue<string>();
        Avatar = avatar ?? Optional.NoValue<string>();
        Active = active ?? Optional.NoValue<bool>();
        AutoReply = autoReply ?? Optional.NoValue<AutoReply>();
    }

    public string Id { get; }
    public Optional<string> Name { get; }
    public Optional<string> Avatar { get; }
    public Optional<bool> Active { get; }
    public Optional<AutoReply> AutoReply { get; }
}
