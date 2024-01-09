using Webinex.Chatify.Abstractions;
using Webinex.Chatify.Abstractions.Events;
using Webinex.Chatify.Common;
using Webinex.Chatify.DataAccess;
using Webinex.Chatify.Rows;

namespace Webinex.Chatify.Services.Subscribers;

[EventSubscriberPriority(-1000)]
internal class ChatCreatedSubscriber : IEventSubscriber<IEnumerable<ChatCreatedEvent>>
{
    private readonly ChatifyDbContext _dbContext;
    private readonly IEventService _eventService;

    public ChatCreatedSubscriber(ChatifyDbContext dbContext, IEventService eventService)
    {
        _dbContext = dbContext;
        _eventService = eventService;
    }

    public async Task InvokeAsync(IEnumerable<ChatCreatedEvent> events)
    {
        events = events.ToArray();

        var members = events.SelectMany(NewMembers).ToArray();
        await _dbContext.Members.AddRangeAsync(members);

        var messages = events.Select(NewMessage);
        await _dbContext.Messages.AddRangeAsync(messages);

        await _eventService.FlushAsync();
    }

    private MessageRow NewMessage(ChatCreatedEvent @event)
    {
        var (authorId, message) = @event.Message != null
            ? (@event.CreatedBy.Id, @event.Message)
            : (AccountContext.System.Id, MessageBody.ChatCreated());

        return MessageRow.NewChatCreated(
            _eventService,
            @event.Id,
            @event.Name,
            @event.Members,
            authorId,
            message,
            requestId: @event.RequestId);
    }

    private MemberRow[] NewMembers(ChatCreatedEvent @event)
    {
        return @event.Members.Select(member => NewMember(@event, member)).ToArray();
    }

    private MemberRow NewMember(ChatCreatedEvent @event, string memberId)
    {
        return MemberRow.NewInitial(_eventService, @event.Id, memberId, @event.CreatedBy.Id, @event.CreatedAt);
    }
}