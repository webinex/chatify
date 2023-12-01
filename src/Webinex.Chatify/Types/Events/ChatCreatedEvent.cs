using Webinex.Chatify.Abstractions;

namespace Webinex.Chatify.Types.Events;

public record ChatCreatedEvent(
    Guid Id,
    string Name,
    AccountContext CreatedBy,
    DateTimeOffset CreatedAt,
    IEnumerable<string>? Members,
    NewMessage Message,
    string? RequestId);