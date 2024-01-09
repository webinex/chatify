namespace Webinex.Chatify.Abstractions.Events;

public record ReadEvent(Guid ChatId, string MessageId, string ToId);