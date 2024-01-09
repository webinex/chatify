using Microsoft.AspNetCore.SignalR;
using Webinex.Chatify.Abstractions;
using Webinex.Chatify.Abstractions.Events;

namespace Webinex.Chatify.AspNetCore;

[EventSubscriberPriority(-500)]
internal class ChatifySignalREventSubscriber<THub>
    : IEventSubscriber<IEnumerable<NewChatMessageDeliveryCreatedEvent>>,
        IEventSubscriber<IEnumerable<MessageSentDeliveryCreatedEvent>>,
        IEventSubscriber<IEnumerable<ReadEvent>>,
        IEventSubscriber<IEnumerable<MemberAddedEvent>>,
        IEventSubscriber<IEnumerable<MemberRemovedEvent>> where THub : ChatifyHub
{
    private readonly IHubContext<THub> _hub;
    private readonly IChatifyHubConnections _connections;
    private readonly IChatify _chatify;

    public ChatifySignalREventSubscriber(IHubContext<THub> hub, IChatifyHubConnections connections, IChatify chatify)
    {
        _hub = hub;
        _connections = connections;
        _chatify = chatify;
    }

    public async Task InvokeAsync(IEnumerable<NewChatMessageDeliveryCreatedEvent> events)
    {
        events = events.ToArray();
        events = events.Where(x => _connections.Connected(x.ToId)).ToArray();
        var accountById = await _chatify.AccountByIdAsync(events.Select(x => x.FromId));

        foreach (var x in events)
        {
            var message = new MessageDto(
                x.MessageId,
                x.Chat.Id,
                x.Body.Text,
                x.SentAt,
                x.Body.Files,
                new AccountDto(accountById[x.FromId]),
                x.Read);

            var chatListItem = new ChatListItemDto(
                x.Chat.Id,
                x.Chat.Name,
                message,
                unreadCount: message.Read ? 0 : 1);

            await _hub.Clients.User(x.ToId).SendCoreAsync(
                "chatify://chat-created",
                [chatListItem, x.RequestId]);
        }
    }

    public async Task InvokeAsync(IEnumerable<MessageSentDeliveryCreatedEvent> events)
    {
        events = events.ToArray();
        events = events.Where(x => _connections.Connected(x.ToId)).ToArray();
        var accountById = await _chatify.AccountByIdAsync(events.Select(x => x.FromId));

        foreach (var x in events)
        {
            var message = new MessageDto(x.MessageId, x.ChatId, x.Body.Text, x.SentAt, x.Body.Files,
                new AccountDto(accountById[x.FromId]), x.Read);

            await _hub.Clients.User(x.ToId).SendCoreAsync(
                "chatify://new-message",
                [message, x.RequestId]);
        }
    }

    public async Task InvokeAsync(IEnumerable<ReadEvent> events)
    {
        var grouped = events.Where(x => _connections.Connected(x.ToId)).GroupBy(x => x.ToId).ToArray();

        foreach (var x in grouped)
        {
            await _hub.Clients.User(x.Key).SendCoreAsync(
                "chatify://read",
                [x.Select(e => new { e.MessageId, e.ChatId }).ToArray()]);
        }
    }

    public async Task InvokeAsync(IEnumerable<MemberAddedEvent> events)
    {
        events = events.ToArray();
        var accountById = await _chatify.AccountByIdAsync(ids: events.Select(x => x.AccountId).Distinct());
        var members = await _chatify.MembersAsync(events.Select(x => x.ChatId).Distinct());
        var chats = await _chatify.ChatByIdAsync(chatIds: events.Select(x => x.ChatId));
        var chatById = chats.ToDictionary(x => x.Id);

        foreach (var x in events)
        {
            var chatMembers = members[x.ChatId].Select(m => m.AccountId).Concat(new[] { x.AccountId }).Distinct();

            foreach (var member in chatMembers)
            {
                if (!_connections.Connected(member))
                    continue;

                var message = new MessageDto(x.Message.Id, x.Message.ChatId, x.Message.Body.Text, x.Message.SentAt,
                    x.Message.Body.Files, AccountDto.System(), true);
                
                await _hub.Clients.User(member).SendCoreAsync(
                    "chatify://member-added",
                    [x.ChatId, chatById[x.ChatId].Name, new AccountDto(accountById[x.AccountId]), message]);
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
                    [x.ChatId, x.AccountId, x.DeleteHistory]);
            }
        }
    }
}
