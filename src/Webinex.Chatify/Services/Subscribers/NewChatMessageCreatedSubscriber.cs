using Webinex.Chatify.Abstractions.Events;
using Webinex.Chatify.Common;
using Webinex.Chatify.DataAccess;
using Webinex.Chatify.Rows;

namespace Webinex.Chatify.Services.Subscribers;

[EventSubscriberPriority(-1000)]
internal class NewChatMessageCreatedSubscriber : IEventSubscriber<IEnumerable<NewChatMessageCreatedEvent>>
{
    private readonly IEventService _eventService;
    private readonly ChatifyDbContext _dbContext;

    public NewChatMessageCreatedSubscriber(
        IEventService eventService,
        ChatifyDbContext dbContext)
    {
        _eventService = eventService;
        _dbContext = dbContext;
    }

    public async Task InvokeAsync(IEnumerable<NewChatMessageCreatedEvent> eventEnumerable)
    {
        var events = eventEnumerable.ToArray();
        await AddDeliveriesAsync(events);
        await AddChatActivitiesAsync(events);
    }

    private async Task AddDeliveriesAsync(NewChatMessageCreatedEvent[] events)
    {
        var deliveries = events.SelectMany(x => x.Chat.Members,
            (x, recipient) => DeliveryRow.NewChatMessage(
                _eventService,
                x.Chat.Id,
                x.Chat.Name,
                x.Id,
                x.AuthorId,
                recipient,
                x.Body,
                x.SentAt,
                x.RequestId));

        await _dbContext.Deliveries.AddRangeAsync(deliveries);
    }

    private async Task AddChatActivitiesAsync(NewChatMessageCreatedEvent[] events)
    {
        var activities = events
            .SelectMany(
                x => x.Chat.Members,
                (x, recipient) =>
                    new ChatActivityRow(x.Chat.Id, recipient, x.AuthorId, x.Id));

        await _dbContext.ChatActivities.AddRangeAsync(activities);
    }
}
