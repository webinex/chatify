using Webinex.Chatify.Abstractions.Events;
using Webinex.Chatify.Common;
using Webinex.Chatify.DataAccess;
using Webinex.Chatify.Rows;

namespace Webinex.Chatify.Services.Subscribers;

[EventSubscriberPriority(-1000)]
internal class MessageSentSubscriber : IEventSubscriber<IEnumerable<MessageSentEvent>>
{
    private readonly IMemberService _memberService;
    private readonly IEventService _eventService;
    private readonly ChatifyDbContext _dbContext;

    public MessageSentSubscriber(IMemberService memberService, IEventService eventService, ChatifyDbContext dbContext)
    {
        _memberService = memberService;
        _eventService = eventService;
        _dbContext = dbContext;
    }

    public async Task InvokeAsync(IEnumerable<MessageSentEvent> eventEnumerable)
    {
        var events = eventEnumerable.ToArray();
        var chatIds = events.Select(x => x.ChatId).ToArray();
        var memberIdsByChatId = await _memberService.IdByChatIdAsync(chatIds);

        await AddDeliveriesAsync(events, memberIdsByChatId);
        UpdateChatActivities(events, memberIdsByChatId);
    }

    private async Task AddDeliveriesAsync(MessageSentEvent[] events, IReadOnlyDictionary<Guid, string[]> membersByChatId)
    {
        var deliveries = events.SelectMany(x => membersByChatId[x.ChatId],
            (x, recipient) => DeliveryRow.NewSent(
                _eventService,
                x.ChatId,
                x.Id,
                x.AuthorId,
                recipient,
                x.Body,
                x.SentAt,
                x.RequestId));

        await _dbContext.Deliveries.AddRangeAsync(deliveries);
    }

    private void UpdateChatActivities(
        MessageSentEvent[] events,
        IReadOnlyDictionary<Guid, string[]> memberByChatId)
    {
        var updateChatActivities = events
            .Where(x => x.AuthorId != AccountId.SYSTEM)
            .SelectMany(
                x => memberByChatId[x.ChatId],
                (x, recipient) =>
                    new ChatActivityRow(x.ChatId, recipient, x.AuthorId, x.Id));

        _dbContext.ChatActivities.UpdateRange(updateChatActivities);
    }
}
