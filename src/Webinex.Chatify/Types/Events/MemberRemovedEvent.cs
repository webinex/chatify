namespace Webinex.Chatify.Types.Events;

public record MemberRemovedEvent(Guid ChatId, string AccountId, bool DeleteHistory);