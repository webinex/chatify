using Microsoft.AspNetCore.SignalR;
using Webinex.Chatify.Abstractions;
using Webinex.Chatify.AspNetCore.Controller;
using Webinex.Chatify.Common.Events;
using Webinex.Chatify.Types.Events;

namespace Webinex.Chatify.AspNetCore.SignalR;

internal class ChatifySignalREventHandler<THub>
    : ISubscriber<IEnumerable<DeliveryCreatedEvent>>,
        ISubscriber<IEnumerable<ReadEvent>>,
        ISubscriber<IEnumerable<MemberAddedEvent>>,
        ISubscriber<IEnumerable<MemberRemovedEvent>> where THub : ChatifyHub
{
    private readonly IHubContext<THub> _hub;
    private readonly IChatifyHubConnections _connections;
    private readonly IChatify _chatify;

    public ChatifySignalREventHandler(IHubContext<THub> hub, IChatifyHubConnections connections, IChatify chatify)
    {
        _hub = hub;
        _connections = connections;
        _chatify = chatify;
    }

    public async Task InvokeAsync(IEnumerable<DeliveryCreatedEvent> events)
    {
        events = events.ToArray();
        events = events.Where(x => _connections.Connected(x.ToId)).ToArray();
        var accounts = await _chatify.AccountByIdAsync(events.Select(x => x.FromId));
        var accountById = accounts.ToDictionary(x => x.Id);

        foreach (var x in events)
        {
            var message = new MessageDto(x.MessageId, x.ChatId, x.Content.Text, x.SentAt, x.Content.Files,
                new AccountDto(accountById[x.FromId]), x.Read);

            if (x.ChatCreatedEvent == null)
            {
                await _hub.Clients.User(x.ToId).SendCoreAsync(
                    "chatify://new-message",
                    new object?[] { message, x.RequestId });
            }
            else
            {
                var chatListItemDto = new ChatListItemDto(x.ChatCreatedEvent.Id, x.ChatCreatedEvent.Name, message,
                    message.Read ? 0 : 1);
                await _hub.Clients.User(x.ToId).SendCoreAsync(
                    "chatify://chat-created",
                    new object?[] { chatListItemDto, x.RequestId });
            }
        }
    }

    public async Task InvokeAsync(IEnumerable<ReadEvent> events)
    {
        var grouped = events.Where(x => _connections.Connected(x.ToId)).GroupBy(x => x.ToId).ToArray();

        foreach (var x in grouped)
        {
            await _hub.Clients.User(x.Key).SendCoreAsync(
                "chatify://read",
                new object?[] { x.Select(e => new { e.MessageId, e.ChatId }).ToArray() });
        }
    }

    public async Task InvokeAsync(IEnumerable<MemberAddedEvent> events)
    {
        events = events.ToArray();
        var accounts = await _chatify.AccountByIdAsync(ids: events.Select(x => x.AccountId).Distinct());
        var members = await _chatify.MembersAsync(events.Select(x => x.ChatId).Distinct());

        foreach (var x in events)
        {
            var chatMembers = members[x.ChatId].Select(m => m.AccountId).Distinct();
            var account = accounts.First(a => a.Id == x.AccountId);

            foreach (var member in chatMembers)
            {
                if (!_connections.Connected(member))
                    continue;

                await _hub.Clients.User(member).SendCoreAsync(
                    "chatify://member-added",
                    new object?[] { x.ChatId, new AccountDto(account) });
            }
        }
    }

    public async Task InvokeAsync(IEnumerable<MemberRemovedEvent> events)
    {
        events = events.ToArray();
        var members = await _chatify.MembersAsync(events.Select(x => x.ChatId).Distinct());

        foreach (var x in events)
        {
            var chatMembers = members[x.ChatId].Select(m => m.AccountId).Concat(new[] { x.AccountId }).Distinct();

            foreach (var member in chatMembers)
            {
                if (!_connections.Connected(member))
                    continue;

                await _hub.Clients.User(member).SendCoreAsync(
                    "chatify://member-removed",
                    new object?[] { x.ChatId, x.AccountId, x.DeleteHistory });
            }
        }
    }
}