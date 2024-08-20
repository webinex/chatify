using Webinex.Chatify.Abstractions;

namespace Webinex.Chatify.Rows.Chats;

internal class ChatMemberRow
{
    private ChatMemberAbstraction _abstraction;

    public Guid Id { get; protected init; }
    public Guid ChatId { get; protected set; }
    public string AccountId { get; protected init; } = null!;
    public string AddedById { get; protected init; } = null!;
    public DateTimeOffset AddedAt { get; protected init; }
    public string FirstMessageId { get; protected init; } = null!;
    public string? LastMessageId { get; protected set; }

    public int FirstMessageIndex() => ChatMessageId.Parse(FirstMessageId).Index;
    public int? LastMessageIndex() => LastMessageId != null ? ChatMessageId.Parse(LastMessageId).Index : null;

    protected ChatMemberRow()
    {
        _abstraction = new ChatMemberAbstraction(this);
    }

    internal ChatMemberRow(
        Guid id,
        Guid chatId,
        string accountId,
        string addedById,
        DateTimeOffset addedAt,
        string firstMessageId,
        string? lastMessageId)
        : this()
    {
        Id = id;
        ChatId = chatId;
        AccountId = accountId ?? throw new ArgumentNullException(nameof(accountId));
        AddedById = addedById ?? throw new ArgumentNullException(nameof(addedById));
        AddedAt = addedAt;
        FirstMessageId = firstMessageId ?? throw new ArgumentNullException(nameof(firstMessageId));
        LastMessageId = lastMessageId;
    }

    internal static ChatMemberRow NewInitial(ChatRow chatRow, ChatMessageRow chatMessageRow, string id)
    {
        return NewInitial(chatRow.Id, id, chatRow.CreatedById, chatRow.CreatedAt, chatMessageRow.Id);
    }

    internal static ChatMemberRow NewInitial(
        Guid chatId,
        string accountId,
        string addedById,
        DateTimeOffset addedAt,
        string firstMessageId)
    {
        if (accountId == AccountContext.System.Id)
            throw new ArgumentException("Cannot add system account to chat", nameof(accountId));

        var member = new ChatMemberRow(Guid.NewGuid(), chatId, accountId, addedById, addedAt, firstMessageId, null);
        return member;
    }

    public static ChatMemberRow NewAdded(
        Guid chatId,
        string accountId,
        string addedById,
        string firstMessageId)
    {
        return new ChatMemberRow(Guid.NewGuid(), chatId, accountId, addedById, DateTimeOffset.UtcNow, firstMessageId, null);
    }

    public ChatMember ToAbstraction()
    {
        return _abstraction;
    }

    private class ChatMemberAbstraction : ChatMember
    {
        private readonly ChatMemberRow _row;

        public override Guid ChatId => _row.ChatId;
        public override string AccountId => _row.AccountId;
        public override string AddedById => _row.AddedById;
        public override DateTimeOffset AddedAt => _row.AddedAt;

        public ChatMemberAbstraction(ChatMemberRow row)
        {
            _row = row;
        }
    }
}
