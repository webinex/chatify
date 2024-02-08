namespace Webinex.Chatify.Abstractions.Events;

public record MemberRemovedEvent(Guid ChatId, string AccountId, bool DeleteHistory, Message Message);