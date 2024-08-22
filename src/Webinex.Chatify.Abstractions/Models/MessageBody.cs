namespace Webinex.Chatify.Abstractions;

public class MessageBody : Equatable
{
    public string? Text { get; }
    public IReadOnlyCollection<File> Files { get; }

    public MessageBody(string? text, IReadOnlyCollection<File> files)
    {
        if (text == null && files.Count == 0)
            throw new ArgumentException("At least one of text or files must be provided.");
        
        Text = text;
        Files = files;
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Text;
        foreach (var file in Files) yield return file;
    }
}
