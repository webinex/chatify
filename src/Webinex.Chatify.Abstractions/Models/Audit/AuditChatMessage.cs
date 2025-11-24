namespace Webinex.Chatify.Abstractions.Audit;

public class AuditChatMessage
{
    public string Id { get; }
    public Guid ChatId { get; }
    public string AuthorId { get; }
    public DateTimeOffset SentAt { get; }
    public string? Text { get; }
    public IReadOnlyCollection<File> Files { get; }
    public Account SentBy { get; }

    public AuditChatMessage(
        string id,
        Guid chatId,
        string authorId,
        DateTimeOffset sentAt,
        Account sentBy,
        string? text,
        IReadOnlyCollection<File> files)
    {
        Id = id;
        ChatId = chatId;
        AuthorId = authorId;
        SentAt = sentAt;
        SentBy = sentBy;
        Text = text;
        Files = files;
    }
}
