using Thread = Webinex.Chatify.Abstractions.Thread;

namespace Webinex.Chatify.AspNetCore.Threads;

public class ThreadListItemDto
{
    public string Id { get; }
    public string Name { get; }
    public string CreatedById { get; }
    public DateTimeOffset CreatedAt { get; }
    public bool Archive { get; }
    public ThreadMessageDto? LastMessage { get; }
    public string? LastReadMessageId { get; }

    public ThreadListItemDto(Thread thread, ThreadMessageDto? lastMessage)
    {
        Id = thread.Id;
        Name = thread.Name;
        CreatedById = thread.CreatedById;
        CreatedAt = thread.CreatedAt;
        Archive = thread.Archived;
        LastMessage = lastMessage;
        LastReadMessageId = thread.LastReadMessageId.Value;
    }

    public ThreadListItemDto(
        string id,
        string name,
        string createdById,
        DateTimeOffset createdAt,
        bool archive,
        ThreadMessageDto? lastMessage,
        string? lastReadMessageId)
    {
        Id = id;
        Name = name;
        CreatedById = createdById;
        CreatedAt = createdAt;
        Archive = archive;
        LastMessage = lastMessage;
        LastReadMessageId = lastReadMessageId;
    }
}
