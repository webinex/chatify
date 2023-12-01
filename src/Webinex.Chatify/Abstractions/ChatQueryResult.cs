using Webinex.Chatify.Types;

namespace Webinex.Chatify.Abstractions;

public class ChatQueryResult
{
    public ChatQueryResult(
        ChatQuery query,
        Chat[] chats,
        IDictionary<Chat, LastMessage>? lastMessages,
        IDictionary<Chat, int>? unreadCount)
    {
        Query = query;
        Chats = chats;
        LastMessages = lastMessages;
        UnreadCount = unreadCount;
    }

    public ChatQuery Query { get; }

    public Chat[] Chats { get; }
    public IDictionary<Chat, LastMessage>? LastMessages { get; }
    public IDictionary<Chat, int>? UnreadCount { get; }

    public record LastMessage(Message Message, Delivery Delivery, Account Author);
}