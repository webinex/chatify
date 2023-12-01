using Webinex.Chatify.Abstractions;

namespace Webinex.Chatify.Types.Events;

public record MessageCreatedEvent(
    string Id,
    Guid ChatId,
    MessageContent Content,
    string AuthorId,
    IEnumerable<string> Recipients,
    DateTimeOffset SentAt,
    ChatCreatedEvent? ChatCreatedEvent,
    string? RequestId);