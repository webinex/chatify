namespace Webinex.Chatify.Abstractions.Events;

public record ThreadMessageSendEvent(string MessageId, string ThreadId, MessageBody Body, AccountContext SentBy, DateTimeOffset SentAt, string? ReadForId);
