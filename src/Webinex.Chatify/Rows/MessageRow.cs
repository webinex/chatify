using System.Linq.Expressions;
using File = Webinex.Chatify.Abstractions.File;

namespace Webinex.Chatify.Rows;

internal class MessageRow
{
    public string Id { get; protected init; } = null!;
    public Guid ChatId { get; protected init; }
    public string Text { get; protected init; } = null!;
    public string AuthorId { get; protected init; } = null!;
    public DateTimeOffset SentAt { get; protected init; }
    public int Index { get; protected init; }
    public IReadOnlyCollection<File> Files { get; protected init; } = null!;
    public virtual AccountRow Author { get; protected init; } = null!;

    internal MessageRow(
        string id,
        Guid chatId,
        string authorId,
        DateTimeOffset sentAt,
        string text,
        IReadOnlyCollection<File> files)
    {
        Id = id;
        ChatId = chatId;
        AuthorId = authorId;
        SentAt = sentAt;
        Text = text ?? throw new ArgumentNullException(nameof(text));
        Files = files ?? throw new ArgumentNullException(nameof(files));
    }

    protected MessageRow()
    {
    }

    public static string FormatSqlInsert(
        (string IdRef, string ChatIdRef, string TextRef, string AuthorIdRef, string SentAtRef, string IndexRef, string? FilesRef) args)
    {
        return $"""
                insert into chatify.Messages(Id, ChatId, Text, AuthorId, SentAt, [Index], Files)
                values ({args.IdRef}, {args.ChatIdRef}, {args.TextRef}, {args.AuthorIdRef}, {args.SentAtRef}, {args.IndexRef}, {args.FilesRef ?? "'[]'"})
                """;
    }

    public static Expression<Func<MessageRow, bool>> In(IQueryable<MemberRow> members)
    {
        return message => members.Any(member =>
            member.FirstMessageId.CompareTo(message.Id) <= 0 &&
            (member.LastMessageId == null || member.LastMessageId.CompareTo(message.Id) >= 0));
    }
}
