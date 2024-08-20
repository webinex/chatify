namespace Webinex.Chatify.Abstractions.Events;

public record ChatMessageSentEvent(
    string Id,
    Guid ChatId,
    MessageBody Body,
    string AuthorId,
    DateTimeOffset SentAt,
    string? RequestId);
