namespace Webinex.Chatify.Abstractions.Events;

public record MessageSentEvent(
    string Id,
    Guid ChatId,
    MessageBody Body,
    string AuthorId,
    DateTimeOffset SentAt,
    string? RequestId,
    bool IsAutoReply = false);
