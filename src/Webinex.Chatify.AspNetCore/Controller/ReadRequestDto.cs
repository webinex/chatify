namespace Webinex.Chatify.AspNetCore.Controller;

public class ReadRequestDto
{
    public ReadRequestDto(IReadOnlyCollection<string> ids)
    {
        Ids = ids;
    }

    public IReadOnlyCollection<string> Ids { get; }
}