namespace Webinex.Chatify.Abstractions;

public class ThreadMessage
{
    public string Id { get; }
    public string ThreadId { get; }
    public string SentById { get; }
    public DateTimeOffset SentAt { get; }
    public string? Text { get; }
    public IReadOnlyCollection<File> Files { get; }

    public Optional<bool?> Read { get; }
    public Optional<Account> SentBy { get; }
    public Optional<Thread> Thread { get; }

    public ThreadMessage(
        string id,
        string threadId,
        string sentById,
        DateTimeOffset sentAt,
        string? text,
        IEnumerable<File>? files,
        Optional<bool?> read,
        Optional<Account>? sentBy = null,
        Optional<Thread>? thread = null)
    {
        Id = id ?? throw new ArgumentNullException(nameof(id));
        ThreadId = threadId ?? throw new ArgumentNullException(nameof(threadId));
        SentById = sentById ?? throw new ArgumentNullException(nameof(sentById));
        SentAt = sentAt;
        Text = text;
        Files = files != null ? files.ToArray() : Array.Empty<File>();
        Read = read;
        SentBy = sentBy ?? Optional.NoValue<Account>();
        Thread = thread ?? Optional.NoValue<Thread>();
    }
    
    [Flags]
    public enum Props
    {
        Default = 0,
        Read = 1,
        SentBy = 2,
        Thread = 4,
    }
}
