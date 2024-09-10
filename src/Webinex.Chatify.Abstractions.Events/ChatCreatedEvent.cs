namespace Webinex.Chatify.Abstractions.Events;

public record ChatCreatedEvent(
    Guid Id,
    string Name,
    AccountContext CreatedBy,
    DateTimeOffset CreatedAt,
    string[] Members,
    MessageBody? Message);