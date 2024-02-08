using Webinex.Chatify.Abstractions;

namespace Webinex.Chatify.AspNetCore;

public class ChatListItemDto
{
    public Guid Id { get; }
    public string Name { get; }
    public MessageDto Message { get; }
    public int TotalUnreadCount { get; }
    public bool Active { get; }
    public string? LastReadMessageId { get; }

    public ChatListItemDto(Chat chat, MessageDto message, int totalUnreadCount)
    {
        Id = chat.Id;
        Name = chat.Name;
        Message = message;
        TotalUnreadCount = totalUnreadCount;
        Active = chat.Active.Value;
        LastReadMessageId = chat.LastReadMessageId.Value;
    }

    public ChatListItemDto(Guid id, string name, MessageDto message, int totalUnreadCount, bool active, string? lastReadMessageId)
    {
        Id = id;
        Name = name;
        Message = message;
        TotalUnreadCount = totalUnreadCount;
        Active = active;
        LastReadMessageId = lastReadMessageId;
    }
}
