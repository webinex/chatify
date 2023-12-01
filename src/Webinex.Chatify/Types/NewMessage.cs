namespace Webinex.Chatify.Types;

public class NewMessage : Equatable
{
    public NewMessage(string text, IReadOnlyCollection<File> files, string authorId)
    {
        Text = text;
        Files = files;
        AuthorId = authorId;
    }

    public string Text { get; protected init; }
    public IReadOnlyCollection<File> Files { get; protected init; }
    public string AuthorId { get; }
    
    public static NewMessage ChatCreated()
    {
        return new NewMessage("chatify://chat-created", Array.Empty<File>(), "chatify::system");
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Text;
        yield return AuthorId;

        foreach (var file in Files)
            yield return file;
    }
}