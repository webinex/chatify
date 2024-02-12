using Microsoft.AspNetCore.SignalR;
using Webinex.Chatify.Abstractions;
using Webinex.Chatify.Abstractions.Events;

namespace Webinex.Chatify.AspNetCore;

[EventSubscriberPriority(-500)]
internal class ChatifySignalREventSubscriber<THub>
    : IEventSubscriber<IEnumerable<NewChatMessageCreatedEvent>>,
        IEventSubscriber<IEnumerable<MessageSentEvent>>,
        IEventSubscriber<IEnumerable<ReadEvent>>,
        IEventSubscriber<IEnumerable<MemberAddedEvent>>,
        IEventSubscriber<IEnumerable<MemberRemovedEvent>>,
        IEventSubscriber<IEnumerable<ChatNameChangedEvent>> where THub : ChatifyHub
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

    public async Task InvokeAsync(IEnumerable<NewChatMessageCreatedEvent> events)
    {
        events = events.ToArray();
        events = events.Where(x => x.Chat.Members.Any(_connections.Connected)).ToArray();
        var accountById = await _chatify.AccountByIdAsync(events.SelectMany(x => x.Chat.Members));

        foreach (var x in events)
        {
            var message = new MessageDto(
                x.Id,
                x.Chat.Id,
                x.Body.Text,
                x.SentAt,
                x.Body.Files,
                sentBy: x.AuthorId == AccountContext.System.Id
                    ? AccountDto.System()
                    : new AccountDto(accountById[x.AuthorId]));

            var chatListItem = new ChatListItemDto(
                x.Chat.Id,
                x.Chat.Name,
                message,
                totalUnreadCount: 1,
                active: true,
                lastReadMessageId: null);

            foreach (var member in x.Chat.Members)
            {
                await _hub.Clients.User(member).SendCoreAsync(
                    "chatify://chat-created",
                    [chatListItem, x.RequestId]);
            }
        }
    }

    public async Task InvokeAsync(IEnumerable<MessageSentEvent> events)
    {
        events = events.ToArray();
        var chatMembersByChatId = await _chatify.ActiveMemberIdByChatIdAsync(events.Select(x => x.ChatId));
        var accountById = await _chatify.AccountByIdAsync(events.SelectMany(x => chatMembersByChatId[x.ChatId]));

        foreach (var sentEvent in events)
        {
            foreach (var recipient in chatMembersByChatId[sentEvent.ChatId])
            {
                var message = new MessageDto(sentEvent.Id, sentEvent.ChatId, sentEvent.Body.Text, sentEvent.SentAt,
                    sentEvent.Body.Files,
                    sentBy: sentEvent.AuthorId == AccountContext.System.Id
                        ? AccountDto.System()
                        : new AccountDto(accountById[sentEvent.AuthorId]));

                await _hub.Clients.User(recipient).SendCoreAsync(
                    "chatify://new-message",
                    [message, sentEvent.RequestId]);
            }
        }
    }

    public async Task InvokeAsync(IEnumerable<ReadEvent> events)
    {
        events = events.Where(x => _connections.Connected(x.AccountId)).ToArray();
        var grouped = events.Where(x => _connections.Connected(x.AccountId)).GroupBy(x => x.AccountId).ToArray();

        foreach (var x in grouped)
        {
            var signals = x.Select(e => new
            {
                e.NewLastReadMessageId,
                e.ChatId,
                e.ReadCount,
            }).ToArray();

            await _hub.Clients.User(x.Key).SendCoreAsync(
                "chatify://read",
                [signals]);
        }
    }

    public async Task InvokeAsync(IEnumerable<MemberAddedEvent> events)
    {
        events = events.ToArray();
        var accountById = await _chatify.AccountByIdAsync(ids: events.Select(x => x.AccountId).Distinct());
        var members = await _chatify.MembersAsync(events.Select(x => x.ChatId).Distinct(), active: true);
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
                    x.Message.Body.Files, AccountDto.System());

                await _hub.Clients.User(member).SendCoreAsync(
                    "chatify://member-added",
                    [x.ChatId, chatById[x.ChatId].Name, new AccountDto(accountById[x.AccountId]), message, x.WithHistory]);
            }
        }
    }

    public async Task InvokeAsync(IEnumerable<MemberRemovedEvent> events)
    {
        events = events.ToArray();
        var members = await _chatify.MembersAsync(events.Select(x => x.ChatId).Distinct(), active: true);

        foreach (var x in events)
        {
            var chatMembers = members[x.ChatId].Select(m => m.AccountId).Concat(new[] { x.AccountId }).Distinct();


            foreach (var member in chatMembers)
            {
                if (!_connections.Connected(member))
                    continue;

                var message = new MessageDto(x.Message.Id, x.Message.ChatId, x.Message.Body.Text, x.Message.SentAt,
                    x.Message.Body.Files, AccountDto.System());

                await _hub.Clients.User(member).SendCoreAsync(
                    "chatify://member-removed",
                    [x.ChatId, x.AccountId, x.DeleteHistory, message]);
            }
        }
    }

    public async Task InvokeAsync(IEnumerable<ChatNameChangedEvent> events)
    {
        events = events.ToArray();
        var members = await _chatify.MembersAsync(events.Select(x => x.ChatId).Distinct(), active: true);

        foreach (var x in events)
        {
            var chatMembers = members[x.ChatId].Select(m => m.AccountId).Distinct().ToArray();

            foreach (var member in chatMembers)
            {
                if (!_connections.Connected(member))
                    continue;

                var message = new MessageDto(x.Message.Id, x.Message.ChatId, x.Message.Body.Text, x.Message.SentAt,
                    x.Message.Body.Files, AccountDto.System());

                await _hub.Clients.User(member).SendCoreAsync(
                    "chatify://chat-name-changed",
                    [x.ChatId, x.NewName, message]);
            }
        }
    }
}
