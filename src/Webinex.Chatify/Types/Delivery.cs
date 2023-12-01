using Webinex.Chatify.Abstractions;
using Webinex.Chatify.Common.Events;
using Webinex.Chatify.Types.Events;

namespace Webinex.Chatify.Types;

public class Delivery
{
    public Guid ChatId { get; protected init; }
    public string MessageId { get; protected init; } = null!;
    public string FromId { get; protected init; } = null!;
    public string ToId { get; protected init; } = null!;
    public bool Read { get; protected set; }

    protected Delivery()
    {
    }

    private Delivery(Guid chatId, string messageId, string fromId, string toId, bool read)
    {
        ChatId = chatId;
        MessageId = messageId;
        FromId = fromId ?? throw new ArgumentNullException(nameof(fromId));
        ToId = toId ?? throw new ArgumentNullException(nameof(toId));
        Read = read;
    }

    internal static Delivery New(
        IEventService eventService,
        Guid chatId,
        string messageId,
        string fromId,
        string toId,
        MessageContent content,
        DateTimeOffset createdAt,
        bool read,
        ChatCreatedEvent? chatCreatedEvent,
        string? requestId)
    {
        var delivery = new Delivery(chatId, messageId, fromId, toId, read);
        eventService.Push(new DeliveryCreatedEvent(delivery.ChatId, delivery.MessageId, delivery.FromId, delivery.ToId,
            content, createdAt, delivery.Read, chatCreatedEvent, requestId));
        return delivery;
    }

    internal void MarkRead(IEventService eventService)
    {
        if (Read)
            return;
        
        Read = true;
        eventService.Push(new ReadEvent(ChatId, MessageId, ToId));
    }
}