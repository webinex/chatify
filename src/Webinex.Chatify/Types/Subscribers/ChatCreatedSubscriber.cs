using Webinex.Chatify.Common.Events;
using Webinex.Chatify.DataAccess;
using Webinex.Chatify.Types.Events;

namespace Webinex.Chatify.Types.Subscribers;

internal class ChatCreatedSubscriber : ISubscriber<IEnumerable<ChatCreatedEvent>>
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

        var members = events
            .Where(x => x.Members != null)
            .SelectMany(chat =>
                chat.Members!.Select(member => Member.New(chat.Id, member, chat.CreatedBy.Id, chat.CreatedAt)))
            .ToArray();

        await _dbContext.Members.AddRangeAsync(members);

        var messages = events
            .Select(chat => Message.New(_eventService, chat.Id, chat.Message.AuthorId, chat.Message.Text, chat.Message.Files,
                chat.Members!, chatCreatedEvent: chat, requestId: chat.RequestId));

        await _dbContext.Messages.AddRangeAsync(messages);

        await _eventService.FlushAsync();
    }
}