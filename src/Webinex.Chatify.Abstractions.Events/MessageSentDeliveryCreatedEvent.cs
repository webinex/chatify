namespace Webinex.Chatify.Abstractions.Events;

public record MessageSentDeliveryCreatedEvent(
    Guid ChatId,
    string MessageId,
    string FromId,
    string ToId,
    MessageBody Body,
    DateTimeOffset SentAt,
    bool Read,
    string? RequestId);
