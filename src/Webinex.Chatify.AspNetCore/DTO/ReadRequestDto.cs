namespace Webinex.Chatify.AspNetCore;

public class ReadRequestDto
{
    public ReadRequestDto(IReadOnlyCollection<string> ids)
    {
        Ids = ids;
    }

    public IReadOnlyCollection<string> Ids { get; }
}