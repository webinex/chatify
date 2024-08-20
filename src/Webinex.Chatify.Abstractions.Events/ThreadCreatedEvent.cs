namespace Webinex.Chatify.Abstractions.Events;

public record ThreadCreatedEvent(
    string ThreadId,
    string Name,
    AccountContext OnBehalfOf,
    IEnumerable<string> Watchers);
