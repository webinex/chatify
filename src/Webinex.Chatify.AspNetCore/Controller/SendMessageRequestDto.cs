using File = Webinex.Chatify.Types.File;

namespace Webinex.Chatify.AspNetCore.Controller;

public class SendMessageRequestDto
{
    public string Text { get; }

    public IReadOnlyCollection<File> Files { get; }

    public string? RequestId { get; }

    public SendMessageRequestDto(string text, IReadOnlyCollection<File> files, string? requestId)
    {
        Text = text;
        Files = files;
        RequestId = requestId;
    }
}