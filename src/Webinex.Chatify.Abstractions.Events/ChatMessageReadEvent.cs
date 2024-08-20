namespace Webinex.Chatify.Abstractions.Events;

public record ChatMessageReadEvent(Guid ChatId, string AccountId, string NewLastReadMessageId, int ReadCount);