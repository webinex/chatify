using Webinex.Chatify.Abstractions;

namespace Webinex.Chatify.AspNetCore;

public class ChatListItemDto
{
    public ChatListItemDto(Chat chat, MessageDto message, int unreadCount)
    {
        Id = chat.Id;
        Name = chat.Name;
        Message = message;
        UnreadCount = unreadCount;
    }

    public ChatListItemDto(Guid id, string name, MessageDto message, int unreadCount)
    {
        Id = id;
        Name = name;
        Message = message;
        UnreadCount = unreadCount;
    }

    public Guid Id { get; }
    public string Name { get; }
    public MessageDto Message { get; }
    public int UnreadCount { get; }
}