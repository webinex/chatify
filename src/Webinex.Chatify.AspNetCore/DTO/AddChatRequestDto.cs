using Webinex.Chatify.Abstractions;

namespace Webinex.Chatify.AspNetCore;

public class AddChatRequestDto
{
    public string Name { get; }
    public IReadOnlyCollection<string> Members { get; }
    public MessageBody? Message { get; }

    public AddChatRequestDto(string name, IReadOnlyCollection<string> members, MessageBody? message)
    {
        Name = name;
        Members = members;
        Message = message;
    }
}