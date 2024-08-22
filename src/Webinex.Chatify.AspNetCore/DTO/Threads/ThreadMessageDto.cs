using Webinex.Chatify.Abstractions;
using File = Webinex.Chatify.Abstractions.File;

namespace Webinex.Chatify.AspNetCore.Threads;

public class ThreadMessageDto
{
    public string Id { get; }
    public string ThreadId { get; }
    public AccountDto SentBy { get; }
    public DateTimeOffset SentAt { get; }
    public string? Text { get; }
    public IReadOnlyCollection<File> Files { get; }

    public ThreadMessageDto(ThreadMessage message, AccountDto sentBy)
    {
        Id = message.Id;
        ThreadId = message.ThreadId;
        SentBy = sentBy;
        SentAt = message.SentAt;
        Text = message.Text;
        Files = message.Files;
    }

    public ThreadMessageDto(string id, string threadId, AccountDto sentBy, DateTimeOffset sentAt, string? text, IReadOnlyCollection<File> files)
    {
        Id = id;
        ThreadId = threadId;
        SentBy = sentBy;
        SentAt = sentAt;
        Text = text;
        Files = files;
    }
}
