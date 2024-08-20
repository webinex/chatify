namespace Webinex.Chatify.Abstractions.Events;

public record ChatMemberAddedEvent(Guid ChatId, string AccountId, ChatMessage ChatMessage, bool WithHistory, string? ReadForId);