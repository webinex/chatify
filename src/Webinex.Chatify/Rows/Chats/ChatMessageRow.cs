using System.Linq.Expressions;
using Webinex.Chatify.Abstractions;
using File = Webinex.Chatify.Abstractions.File;

namespace Webinex.Chatify.Rows.Chats;

internal class ChatMessageRow
{
    public string Id { get; protected init; } = null!;
    public Guid ChatId { get; protected init; }
    public string? Text { get; protected init; }
    public string AuthorId { get; protected init; } = null!;
    public DateTimeOffset SentAt { get; protected init; }
    public IReadOnlyCollection<File> Files { get; protected init; } = [];
    public AccountRow Author { get; protected init; } = null!;

    internal ChatMessageRow(
        string id,
        Guid chatId,
        string authorId,
        DateTimeOffset sentAt,
        string? text,
        IReadOnlyCollection<File> files)
    {
        Id = id;
        ChatId = chatId;
        AuthorId = authorId;
        SentAt = sentAt;
        Text = text;
        Files = files ?? throw new ArgumentNullException(nameof(files));
    }

    protected ChatMessageRow()
    {
    }

    public static Expression<Func<ChatMessageRow, bool>> In(IQueryable<ChatMemberRow> members)
    {
        return message => members.Any(member =>
            member.ChatId == message.ChatId &&
            member.FirstMessageId.CompareTo(message.Id) <= 0 &&
            (member.LastMessageId == null || member.LastMessageId.CompareTo(message.Id) >= 0));
    }

    public static ChatMessageRow NewMemberRemoved(Guid chatId, int index)
    {
        return new ChatMessageRow(
            ChatMessageId.New(chatId, index).ToString(),
            chatId,
            AccountContext.System.Id,
            DateTimeOffset.UtcNow,
            ChatMessage.WellKnown.MemberRemoved,
            Array.Empty<File>());
    }

    public static ChatMessageRow NewMemberAdded(Guid chatId, int index)
    {
        return new ChatMessageRow(
            ChatMessageId.New(chatId, index).ToString(),
            chatId,
            AccountContext.System.Id,
            DateTimeOffset.UtcNow,
            ChatMessage.WellKnown.MemberAdded,
            Array.Empty<File>());
    }

    public static ChatMessageRow NewChatRenamed(Guid chatId, int index, string newName)
    {
        return new ChatMessageRow(
            ChatMessageId.New(chatId, index).ToString(),
            chatId,
            AccountContext.System.Id,
            DateTimeOffset.UtcNow,
            ChatMessage.WellKnown.ChatRenamed(newName),
            Array.Empty<File>());
    }

    public static ChatMessageRow NewBody(
        Guid chatId,
        int index,
        string authorId,
        string? text,
        IEnumerable<File>? files = null)
    {
        var id = ChatMessageId.New(chatId, index);
        return new ChatMessageRow(id.ToString(), chatId, authorId, DateTimeOffset.UtcNow, text,
            files?.ToArray() ?? Array.Empty<File>());
    }

    public static ChatMessageRow NewChatCreated(
        Guid chatId)
    {
        return new ChatMessageRow(
            ChatMessageId.New(chatId, 0).ToString(),
            chatId,
            AccountContext.System.Id,
            DateTimeOffset.UtcNow,
            ChatMessage.WellKnown.ChatCreated, Array.Empty<File>());
    }
}
