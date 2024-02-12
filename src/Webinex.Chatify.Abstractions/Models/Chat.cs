namespace Webinex.Chatify.Abstractions;

public class Chat
{
    public Guid Id { get; }
    public string Name { get; }
    public DateTimeOffset CreatedAt { get; }
    public string CreatedById { get; }
    public Optional<string?> LastReadMessageId { get; }
    public Optional<bool> Active { get; }
    public Optional<Message> LastMessage { get; }
    public Optional<int> TotalUnreadCount { get; }


    public Chat(
        Guid id,
        string name,
        DateTimeOffset createdAt,
        string createdById,
        Optional<string?> lastReadMessageId,
        Optional<bool> active,
        Optional<Message> lastMessage,
        Optional<int> totalUnreadCount)
    {
        Id = id;
        Name = name;
        CreatedAt = createdAt;
        CreatedById = createdById;
        LastReadMessageId = lastReadMessageId;
        Active = active;
        LastMessage = lastMessage;
        TotalUnreadCount = totalUnreadCount;
    }

    [Flags]
    public enum Props
    {
        Default = 0,
        TotalUnreadCount = 1,
        LastMessage = 2,
        LastMessageRead = LastMessage | 4,
        LastMessageAuthor = LastMessage | 8,
    }
}

public static class ChatPropExtensions
{
    public static bool HasLastMessage(this Chat.Props props)
    {
        return props.HasFlag(Chat.Props.LastMessage);
    }

    public static Message.Props ToLastMessageProps(this Chat.Props props)
    {
        Message.Props messageProps = 0;

        if (props.HasFlag(Chat.Props.LastMessageRead))
            messageProps |= Message.Props.Read;

        if (props.HasFlag(Chat.Props.LastMessageAuthor))
            messageProps |= Message.Props.Author;

        return messageProps;
    }
}
