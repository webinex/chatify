using Webinex.Chatify.Common.Events;
using Webinex.Chatify.DataAccess;
using Webinex.Chatify.Types.Events;

namespace Webinex.Chatify.Types.Subscribers;

internal class MessageCreatedSubscriber : ISubscriber<IEnumerable<MessageCreatedEvent>>
{
    private readonly IEventService _eventService;
    private readonly ChatifyDbContext _chatifyDbContext;

    public MessageCreatedSubscriber(IEventService eventService, ChatifyDbContext chatifyDbContext)
    {
        _eventService = eventService;
        _chatifyDbContext = chatifyDbContext;
    }

    public async Task InvokeAsync(IEnumerable<MessageCreatedEvent> events)
    {
        events = events.ToArray();

        var deliveries = events.SelectMany(x => x.Recipients,
            (x, recipient) => Delivery.New(_eventService, x.ChatId, x.Id, x.AuthorId, recipient,
                x.Content, x.SentAt, x.AuthorId == recipient || x.AuthorId == AccountId.SYSTEM, x.ChatCreatedEvent, x.RequestId));

        var firstChatActivities = events
            .Where(x => x.ChatCreatedEvent != null)
            .SelectMany(x => x.Recipients, (x, recipient) => new ChatActivity(
                x.ChatId,
                recipient,
                x.AuthorId,
                x.Id));

        await _chatifyDbContext.Deliveries.AddRangeAsync(deliveries);
        await _chatifyDbContext.ChatActivities.AddRangeAsync(firstChatActivities);

        var updateChatActivities = events
            .Where(x => x.ChatCreatedEvent == null && x.AuthorId != AccountId.SYSTEM)
            .SelectMany(x => x.Recipients, (x, recipient) => new ChatActivity(x.ChatId, recipient, x.AuthorId, x.Id));

        _chatifyDbContext.ChatActivities.UpdateRange(updateChatActivities);
    }
}