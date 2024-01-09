namespace Webinex.Chatify.Abstractions;

public class MessageBody : Equatable
{
    public string Text { get; }
    public IReadOnlyCollection<File> Files { get; }

    public MessageBody(string text, IReadOnlyCollection<File> files)
    {
        Text = text;
        Files = files;
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Text;
        foreach (var file in Files) yield return file;
    }

    public static MessageBody ChatCreated()
    {
        return new MessageBody("chatify://chat-created", Array.Empty<File>());
    }
}
