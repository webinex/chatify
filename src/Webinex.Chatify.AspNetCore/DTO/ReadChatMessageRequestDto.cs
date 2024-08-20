namespace Webinex.Chatify.AspNetCore;

public class ReadChatMessageRequestDto
{
    public ReadChatMessageRequestDto(string id)
    {
        Id = id;
    }

    public string Id { get; }
}