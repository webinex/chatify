using Webinex.Chatify.Abstractions;

namespace Webinex.Chatify.Types.Events;

public record DeliveryCreatedEvent(
    Guid ChatId,
    string MessageId,
    string FromId,
    string ToId,
    MessageContent Content,
    DateTimeOffset SentAt,
    bool Read,
    ChatCreatedEvent? ChatCreatedEvent,
    string? RequestId);