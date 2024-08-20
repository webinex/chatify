using Thread = Webinex.Chatify.Abstractions.Thread;

namespace Webinex.Chatify.AspNetCore.Threads;

public class ThreadDto
{
    public string Id { get; }
    public string Name { get; }
    public AccountDto CreatedBy { get; }
    public DateTimeOffset CreatedAt { get; }
    public bool Archive { get; }
    public string? LastMessageId { get; }
    public string? LastReadMessageId { get; }
    public bool Watch { get; }

    public ThreadDto(Thread thread, AccountDto createdBy)
    {
        Id = thread.Id;
        Name = thread.Name;
        CreatedBy = createdBy;
        CreatedAt = thread.CreatedAt;
        Archive = thread.Archived;
        LastMessageId = thread.LastMessageId;
        LastReadMessageId = thread.LastReadMessageId.Value;
        Watch = thread.Watch.Value;
    }
}
