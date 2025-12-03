namespace Webinex.Chatify.Abstractions;

public class Chat
{
    public Guid Id { get; }
    public string WorkspaceId { get; }
    public string Name { get; }
    public DateTimeOffset CreatedAt { get; }
    public string CreatedById { get; }
    public Optional<string?> LastReadMessageId { get; }
    public Optional<bool> Active { get; }
    public Optional<ChatMessage> LastMessage { get; }
    public Optional<int> TotalUnreadCount { get; }


    public Chat(
        Guid id,
        string workspaceId,
        string name,
        DateTimeOffset createdAt,
        string createdById,
        Optional<string?> lastReadMessageId,
        Optional<bool> active,
        Optional<ChatMessage> lastMessage,
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
        WorkspaceId = workspaceId;
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

    public static ChatMessage.Props ToLastMessageProps(this Chat.Props props)
    {
        ChatMessage.Props messageProps = 0;

        if (props.HasFlag(Chat.Props.LastMessageRead))
            messageProps |= ChatMessage.Props.Read;

        if (props.HasFlag(Chat.Props.LastMessageAuthor))
            messageProps |= ChatMessage.Props.Author;

        return messageProps;
    }
}
