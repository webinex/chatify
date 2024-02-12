namespace Webinex.Chatify.Abstractions.Events;

public record MemberAddedEvent(Guid ChatId, string AccountId, Message Message, bool WithHistory);