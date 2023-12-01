using Webinex.Chatify.Abstractions;

namespace Webinex.Chatify.AspNetCore.Controller;

public class AddChatRequestDto
{
    public string Name { get; }
    public IReadOnlyCollection<string> Members { get; }
    public MessageContent? Message { get; }
    public string RequestId { get; }

    public AddChatRequestDto(string requestId, string name, IReadOnlyCollection<string> members, MessageContent? message)
    {
        Name = name;
        Members = members;
        Message = message;
        RequestId = requestId;
    }
}