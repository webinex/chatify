using Webinex.Chatify.Abstractions;
using File = Webinex.Chatify.Abstractions.File;

namespace Webinex.Chatify.Rows.Threads;

internal class ThreadMessageRow
{
    public string Id { get; protected init; } = null!;
    public string ThreadId { get; protected init; } = null!;
    public string SentById { get; protected init; } = null!;
    public DateTimeOffset SentAt { get; protected init; }
    public string? Text { get; protected init; }
    public IReadOnlyCollection<File> Files { get; protected init; } = Array.Empty<File>();

    public MessageBody Body() => new(Text, Files);

    public ThreadMessageRow(string id, string threadId, string sentById, DateTimeOffset sentAt, string? text, IEnumerable<File>? files)
    {
        Id = id ?? throw new ArgumentNullException(nameof(id));
        ThreadId = threadId ?? throw new ArgumentNullException(nameof(threadId));
        SentById = sentById ?? throw new ArgumentNullException(nameof(sentById));
        SentAt = sentAt;
        Text = text;
        Files = files != null ? files.ToArray() : Array.Empty<File>();
    }

    protected ThreadMessageRow()
    {
    }

    public static ThreadMessageRow New(ThreadMessageId id, string sentById, MessageBody body)
    {
        return new ThreadMessageRow(id.ToString(), id.ThreadId, sentById, DateTimeOffset.UtcNow, body.Text, body.Files);
    }
}
