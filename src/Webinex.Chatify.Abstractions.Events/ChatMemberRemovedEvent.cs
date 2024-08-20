namespace Webinex.Chatify.Abstractions.Events;

public record ChatMemberRemovedEvent(Guid ChatId, string AccountId, bool DeleteHistory, ChatMessage ChatMessage, string? ReadForId);