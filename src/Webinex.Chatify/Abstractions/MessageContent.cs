using File = Webinex.Chatify.Types.File;

namespace Webinex.Chatify.Abstractions;

public class MessageContent
{
    public string Text { get; }
    public IReadOnlyCollection<File> Files { get; }

    public MessageContent(string text, IReadOnlyCollection<File> files)
    {
        Text = text;
        Files = files;
    }
}