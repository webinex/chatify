using Webinex.Chatify.Abstractions;
using Webinex.Chatify.Abstractions.Events;
using Webinex.Chatify.Common;

namespace Webinex.Chatify.Rows;

internal class DeliveryRow
{
    public Guid ChatId { get; protected init; }
    public string MessageId { get; protected init; } = null!;
    public string FromId { get; protected init; } = null!;
    public string ToId { get; protected init; } = null!;
    public bool Read { get; protected set; }

    public MessageRow? Message { get; protected set; }

    protected DeliveryRow()
    {
    }

    private DeliveryRow(Guid chatId, string messageId, string fromId, string toId, bool read)
    {
        ChatId = chatId;
        MessageId = messageId;
        FromId = fromId ?? throw new ArgumentNullException(nameof(fromId));
        ToId = toId ?? throw new ArgumentNullException(nameof(toId));
        Read = read;
    }

    internal static DeliveryRow NewSent(
        IEventService eventService,
        Guid chatId,
        string messageId,
        string fromId,
        string toId,
        MessageBody body,
        DateTimeOffset createdAt,
        string? requestId)
    {
        if (toId ==  AccountContext.System.Id)
            throw new ArgumentException("Cannot send message to system account", nameof(toId));
        
        var read = fromId == AccountContext.System.Id || fromId == toId;
        var delivery = new DeliveryRow(chatId, messageId, fromId, toId, read);

        eventService.Push(new MessageSentDeliveryCreatedEvent(
            delivery.ChatId,
            delivery.MessageId,
            delivery.FromId,
            delivery.ToId,
            body,
            createdAt,
            delivery.Read,
            requestId));

        return delivery;
    }

    internal static DeliveryRow NewChatMessage(
        IEventService eventService,
        Guid chatId,
        string chatName,
        string messageId,
        string fromId,
        string toId,
        MessageBody body,
        DateTimeOffset createdAt,
        string? requestId)
    {
        if (toId ==  AccountContext.System.Id)
            throw new ArgumentException("Cannot send message to system account", nameof(toId));

        var read = fromId == AccountContext.System.Id || fromId == toId;
        var delivery = new DeliveryRow(chatId, messageId, fromId, toId, read);

        eventService.Push(new NewChatMessageDeliveryCreatedEvent(
            new NewChatMessageDeliveryCreatedEvent.NewChatValue(chatId, chatName),
            delivery.MessageId,
            delivery.FromId,
            delivery.ToId,
            body,
            createdAt,
            delivery.Read,
            requestId));

        return delivery;
    }

    internal static DeliveryRow NewMemberAddedByJob(Guid chatId, string messageId, string recipientId)
    {
        return new DeliveryRow(
            chatId: chatId,
            messageId: messageId,
            fromId: AccountContext.System.Id,
            toId: recipientId,
            read: true);
    }

    internal void MarkRead(IEventService eventService)
    {
        if (Read)
            return;

        Read = true;
        eventService.Push(new ReadEvent(ChatId, MessageId, ToId));
    }
}
