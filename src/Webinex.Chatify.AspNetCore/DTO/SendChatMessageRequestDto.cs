using File = Webinex.Chatify.Abstractions.File;

namespace Webinex.Chatify.AspNetCore;

public class SendChatMessageRequestDto
{
    public string? Text { get; }
    public IReadOnlyCollection<File> Files { get; }

    public SendChatMessageRequestDto(string? text, IReadOnlyCollection<File> files)
    {
        Text = text;
        Files = files;
    }
}