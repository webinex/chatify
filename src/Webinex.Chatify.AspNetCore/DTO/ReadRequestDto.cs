namespace Webinex.Chatify.AspNetCore;

public class ReadRequestDto
{
    public ReadRequestDto(string id)
    {
        Id = id;
    }

    public string Id { get; }
}