namespace Webinex.Chatify.Abstractions.Events;

public record ReadEvent(Guid ChatId, string AccountId, string NewLastReadMessageId, int ReadCount);