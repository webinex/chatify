namespace Webinex.Chatify.Abstractions.Events;

public record NewChatMessageCreatedEvent(
    string MessageId,
    NewChatMessageCreatedEvent.ChatValue Chat,
    MessageBody Body,
    string AuthorId,
    DateTimeOffset SentAt,
    string? ReadForId)
{
    public record ChatValue(Guid Id, string Name, string[] Members);
};
