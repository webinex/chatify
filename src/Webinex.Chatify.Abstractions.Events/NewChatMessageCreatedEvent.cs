namespace Webinex.Chatify.Abstractions.Events;

public record NewChatMessageCreatedEvent(
    string Id,
    NewChatMessageCreatedEvent.ChatValue Chat,
    MessageBody Body,
    string AuthorId,
    DateTimeOffset SentAt,
    string? RequestId)
{
    public record ChatValue(Guid Id, string Name, string[] Members);
};
