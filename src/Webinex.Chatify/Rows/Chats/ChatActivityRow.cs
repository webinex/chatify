namespace Webinex.Chatify.Rows.Chats;

internal class ChatActivityRow
{
    public Guid ChatId { get; protected init; }
    public string AccountId { get; protected init; } = null!;
    public string LastMessageFromId { get; protected set; } = null!;
    public string LastMessageId { get; protected set; } = null!;
    public string? LastReadMessageId { get; protected set; }
    public bool Active { get; protected set; }
    public ChatMessageRow LastChatMessage { get; protected set; } = null!;
    public ChatRow? Chat { get; protected set; } = null!;

    public int LastMessageIndex() => ChatMessageId.Parse(LastMessageId).Index;
    public int? LastReadMessageIndex() => LastReadMessageId != null ? ChatMessageId.Parse(LastReadMessageId).Index : null;

    protected ChatActivityRow()
    {
    }

    internal void Read(ChatMessageId id)
    {
        if (LastReadMessageId != null &&
            string.Compare(LastReadMessageId, id.ToString(), StringComparison.InvariantCultureIgnoreCase) >= 0)
            return;

        LastReadMessageId = id.ToString();
    }

    public ChatActivityRow(
        Guid chatId,
        string accountId,
        string lastMessageFromId,
        string lastMessageId,
        bool active,
        string? lastReadMessageId)
    {
        ChatId = chatId;
        AccountId = accountId;
        LastMessageFromId = lastMessageFromId;
        LastMessageId = lastMessageId;
        LastReadMessageId = lastReadMessageId;
        Active = active;
    }

    public static ChatActivityRow NewInitial(ChatRow chatRow, ChatMessageRow chatMessageRow, string accountId, bool read)
    {
        return new ChatActivityRow(chatRow.Id, accountId, chatMessageRow.AuthorId, chatMessageRow.Id, true, read ? chatMessageRow.Id : null);
    }
}
