namespace Webinex.Chatify.Abstractions.Events;

public record ThreadWatchAddedEvent(string ThreadId, string AccountId, ThreadMessage? LastMessage, string? LastReadMessageId);
