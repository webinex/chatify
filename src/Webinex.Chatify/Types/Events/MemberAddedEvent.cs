namespace Webinex.Chatify.Types.Events;

public record MemberAddedEvent(Guid ChatId, string AccountId);