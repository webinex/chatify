namespace Webinex.Chatify.Abstractions.Events;

public record NewChatMessageDeliveryCreatedEvent(
    NewChatMessageDeliveryCreatedEvent.NewChatValue Chat,
    string MessageId,
    string FromId,
    string ToId,
    MessageBody Body,
    DateTimeOffset SentAt,
    bool Read,
    string? RequestId)
{
    public record NewChatValue(Guid Id, string Name);
}
