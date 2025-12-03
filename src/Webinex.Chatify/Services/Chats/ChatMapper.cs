using Webinex.Chatify.Abstractions;
using Webinex.Chatify.Rows.Chats;
using Webinex.Chatify.Services.Chats.Messages;

namespace Webinex.Chatify.Services.Chats;

internal static class ChatMapper
{
    public static Chat Map(
        ChatRow chat,
        ChatActivityRow? activity = null,
        Chat.Props props = Chat.Props.Default,
        IReadOnlyDictionary<Guid, int>? totalUnreadCountByChatId = null)
    {
        chat = chat ?? throw new ArgumentNullException(nameof(chat));
        var totalUnreadCountRequested = props.HasFlag(Chat.Props.TotalUnreadCount);
        var lastMessageRequested = props.HasLastMessage();

        if (lastMessageRequested && activity?.LastChatMessage == null)
            throw new InvalidOperationException("LastMessage is requested but not loaded");

        if (totalUnreadCountRequested && totalUnreadCountByChatId == null)
            throw new InvalidOperationException("TotalUnreadCount is requested but not provided");

        var lastMessage = props.HasLastMessage()
            ? Optional.Value(ChatMessageMapper.Map(
                chatMessage: activity!.LastChatMessage,
                read: activity.LastReadMessageId?.CompareTo(activity.LastMessageId) >= 0,
                props: props.ToLastMessageProps()))
            : Optional.NoValue<ChatMessage>();

        var totalUnreadCountValue = totalUnreadCountRequested
            ? Optional.Value(totalUnreadCountByChatId![chat.Id])
            : Optional.NoValue<int>();

        return new Chat(
            id: chat.Id,
            workspaceId: chat.WorkspaceId,
            name: chat.Name,
            createdAt: chat.CreatedAt,
            createdById: chat.CreatedById,
            lastReadMessageId: activity != null
                ? Optional.Value<string?>(activity.LastReadMessageId)
                : Optional.NoValue<string?>(),
            active: activity != null
                ? Optional.Value(activity.Active)
                : Optional.NoValue<bool>(),
            lastMessage:
            lastMessage,
            totalUnreadCount:
            totalUnreadCountValue);
    }
}
