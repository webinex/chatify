using Microsoft.AspNetCore.SignalR;
using Webinex.Chatify.Abstractions;
using Webinex.Chatify.Abstractions.Events;
using Webinex.Chatify.AspNetCore.Threads;

namespace Webinex.Chatify.AspNetCore;

[EventSubscriberPriority(-500)]
internal class ChatifyThreadSignalREventSubscriber<THub>
    : IEventSubscriber<IEnumerable<ThreadCreatedEvent>>,
        IEventSubscriber<IEnumerable<ThreadMessageReadEvent>>,
        IEventSubscriber<IEnumerable<ThreadMessageSendEvent>>,
        IEventSubscriber<IEnumerable<ThreadWatchAddedEvent>>,
        IEventSubscriber<IEnumerable<ThreadWatchRemovedEvent>>,
        IEventSubscriber<IEnumerable<ThreadUpdatedEvent>> where THub : ChatifyHub
{
    private readonly IHubContext<THub> _hub;
    private readonly IChatifyHubConnections _connections;
    private readonly IChatify _chatify;

    public ChatifyThreadSignalREventSubscriber(
        IHubContext<THub> hub,
        IChatifyHubConnections connections,
        IChatify chatify)
    {
        _hub = hub;
        _connections = connections;
        _chatify = chatify;
    }

    public async Task InvokeAsync(IEnumerable<ThreadCreatedEvent> events)
    {
        events = events.ToArray();
        var connected = events.Where(x => x.Watchers.Any(_connections.Connected)).ToArray();

        foreach (var @event in connected)
        {
            foreach (var watcher in @event.Watchers)
            {
                if (!_connections.Connected(watcher))
                    continue;

                var thread = new ThreadListItemDto(@event.ThreadId, @event.Name, @event.OnBehalfOf.Id,
                    DateTimeOffset.UtcNow, false, null, null);
                await _hub.Clients.User(watcher).SendCoreAsync("chatify://thread-created", [thread, true]);
            }
        }
    }

    public async Task InvokeAsync(IEnumerable<ThreadMessageReadEvent> events)
    {
        events = events.ToArray();

        foreach (var readEvent in events)
        {
            if (!_connections.Connected(readEvent.ById))
                continue;

            await _hub.Clients.User(readEvent.ById).SendCoreAsync("chatify://thread-message-read",
                [readEvent.ThreadId, readEvent.MessageId]);
        }
    }

    public async Task InvokeAsync(IEnumerable<ThreadMessageSendEvent> events)
    {
        events = events.ToArray();
        var watchersByThreadId = await _chatify.WatchersByThreadIdAsync(events.Select(x => x.ThreadId));
        var accountById = await _chatify.AccountByIdAsync(events.Select(x => x.SentBy.Id));

        foreach (var sendEvent in events)
        {
            var threadMessage = new ThreadMessageDto(sendEvent.MessageId, sendEvent.ThreadId,
                new AccountDto(accountById[sendEvent.SentBy.Id]), sendEvent.SentAt, sendEvent.Body.Text, sendEvent.Body.Files);

            await _hub.Clients.GroupExcept(HubGroupNames.Thread(sendEvent.ThreadId),
                    watchersByThreadId[sendEvent.ThreadId].SelectMany(x => _connections.Get(x)))
                .SendAsync("chatify://thread-message-new", sendEvent.ThreadId, threadMessage, sendEvent.ReadForId);

            foreach (var watcher in watchersByThreadId[sendEvent.ThreadId])
            {
                if (!_connections.Connected(watcher)) continue;
                await _hub.Clients.User(watcher)
                    .SendAsync("chatify://thread-message-new", sendEvent.ThreadId, threadMessage, sendEvent.ReadForId);
            }
        }
    }

    public async Task InvokeAsync(IEnumerable<ThreadWatchAddedEvent> events)
    {
        events = events.Where(x => _connections.Connected(x.AccountId)).ToArray();
        var threads = await _chatify.ThreadByIdAsync(events.Select(x => x.ThreadId));
        var accountById
            = await _chatify.AccountByIdAsync(events.Select(x => x.LastMessage?.SentById).Where(x => x != null)!,
                tryCache: true, required: true);

        foreach (var watchChangedEvent in events)
        {
            var thread = threads[watchChangedEvent.ThreadId]!;
            var message = watchChangedEvent.LastMessage;

            var lastMessageDto = message != null
                ? new ThreadMessageDto(
                    message.Id,
                    message.ThreadId,
                    new AccountDto(accountById[message.SentById]),
                    message.SentAt,
                    message.Text,
                    message.Files)
                : null;

            var threadDto = new ThreadListItemDto(
                thread.Id,
                thread.Name,
                thread.CreatedById,
                thread.CreatedAt,
                thread.Archived,
                lastMessageDto,
                watchChangedEvent.LastReadMessageId);

            await _hub.Clients.User(watchChangedEvent.AccountId).SendAsync("chatify://thread-watch-added", threadDto);
        }
    }

    public async Task InvokeAsync(IEnumerable<ThreadWatchRemovedEvent> events)
    {
        events = events.Where(x => _connections.Connected(x.AccountId)).ToArray();

        foreach (var threadWatchRemovedEvent in events)
        {
            await _hub.Clients.User(threadWatchRemovedEvent.AccountId)
                .SendAsync("chatify://thread-watch-removed", threadWatchRemovedEvent.ThreadId);
        }
    }

    public async Task InvokeAsync(IEnumerable<ThreadUpdatedEvent> events)
    {
        events = events.ToArray();
        var watchersByThreadId = await _chatify.WatchersByThreadIdAsync(events.Select(x => x.Id));

        foreach (var @event in events)
        {
            await _hub.Clients
                .GroupExcept(
                    HubGroupNames.Thread(@event.Id),
                    watchersByThreadId[@event.Id].SelectMany(x => _connections.Get(x))).SendAsync(
                    "chatify://thread-updated",
                    @event.Id,
                    @event.Name);

            foreach (var watcher in watchersByThreadId[@event.Id])
            {
                if (!_connections.Connected(watcher)) continue;
                await _hub.Clients.User(watcher)
                    .SendAsync("chatify://thread-updated", @event.Id, @event.Name);
            }
        }
    }
}