namespace Webinex.Chatify.Types.Events;

public record ReadEvent(Guid ChatId, string MessageId, string ToId);