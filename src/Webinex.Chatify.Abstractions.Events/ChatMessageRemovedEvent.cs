namespace Webinex.Chatify.Abstractions.Events;

public record ChatMessageRemovedEvent(Guid ChatId, string MessageId, string AuthorId, ChatMessage? LastMessage);