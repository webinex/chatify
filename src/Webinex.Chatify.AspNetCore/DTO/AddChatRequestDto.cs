using Webinex.Chatify.Abstractions;

namespace Webinex.Chatify.AspNetCore;

public class AddChatRequestDto
{
    public string Name { get; }
    public IReadOnlyCollection<string> Members { get; }
    public MessageBody? Message { get; }
    public string RequestId { get; }

    public AddChatRequestDto(string requestId, string name, IReadOnlyCollection<string> members, MessageBody? message)
    {
        Name = name;
        Members = members;
        Message = message;
        RequestId = requestId;
    }
}