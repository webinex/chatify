namespace Webinex.Chatify.Abstractions;

public class Thread
{
    public string Id { get; init; }
    public string Name { get; init; }
    public string CreatedById { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
    public bool Archived { get; init; }
    public string? LastMessageId { get; init; }
    
    public Optional<ThreadMessage?> LastMessage { get; init; }
    public Optional<bool> Watch { get; init; }
    public Optional<string?> LastReadMessageId { get; init; }

    public Thread(
        string id,
        string name,
        string createdById,
        DateTimeOffset createdAt,
        bool archived,
        string? lastMessageId,
        Optional<ThreadMessage?>? lastMessage,
        Optional<bool>? watch,
        Optional<string?>? lastReadMessageId)
    {
        Id = id;
        Name = name;
        CreatedById = createdById;
        CreatedAt = createdAt;
        Archived = archived;
        LastMessageId = lastMessageId;
        LastMessage = lastMessage ?? Optional.NoValue<ThreadMessage?>();
        Watch = watch ?? Optional.NoValue<bool>();
        LastReadMessageId = lastReadMessageId ?? Optional.NoValue<string?>();
    }
    
    [Flags]
    public enum Props
    {
        Default = 0,
        LastMessage = 1,
        LastMessageSentBy = LastMessage | 2, 
        Watch = 4,
        LastMessageRead = Watch | 8,
        LastReadMessageId = Watch | 16,
    }
}

public static class ThreadPropExtensions
{
    public static ThreadMessage.Props ToLastMessageProps(this Thread.Props props)
    {
        ThreadMessage.Props messageProps = 0;

        if (props.HasFlag(Thread.Props.LastMessageSentBy))
            messageProps |= ThreadMessage.Props.SentBy;
        
        if (props.HasFlag(Thread.Props.LastMessageRead))
            messageProps |= ThreadMessage.Props.Read;

        return messageProps;
    }
}
