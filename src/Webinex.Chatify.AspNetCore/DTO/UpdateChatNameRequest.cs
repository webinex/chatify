namespace Webinex.Chatify.AspNetCore;

public class UpdateChatNameRequest
{
    public UpdateChatNameRequest(Guid id, string name)
    {
        Id = id;
        Name = name;
    }

    public Guid Id { get; }
    public string Name { get; }
}
