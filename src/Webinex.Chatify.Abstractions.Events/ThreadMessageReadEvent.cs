namespace Webinex.Chatify.Abstractions.Events;

public record ThreadMessageReadEvent(string ThreadId, string MessageId, string ById, int ReadCount);
