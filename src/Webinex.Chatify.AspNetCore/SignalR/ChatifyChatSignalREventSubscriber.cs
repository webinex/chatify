using Microsoft.AspNetCore.SignalR;
using Webinex.Chatify.Abstractions;
using Webinex.Chatify.Abstractions.Events;

namespace Webinex.Chatify.AspNetCore;

[EventSubscriberPriority(-500)]
internal class ChatifyChatSignalREventSubscriber<THub>
    : IEventSubscriber<IEnumerable<NewChatMessageCreatedEvent>>,
        IEventSubscriber<IEnumerable<ChatMessageSentEvent>>,
        IEventSubscriber<IEnumerable<ChatMessageReadEvent>>,
        IEventSubscriber<IEnumerable<ChatMemberAddedEvent>>,
        IEventSubscriber<IEnumerable<ChatMemberRemovedEvent>>,
        IEventSubscriber<IEnumerable<ChatNameChangedEvent>> where THub : ChatifyHub
{
    private readonly IHubContext<THub> _hub;
    private readonly IChatifyHubConnections _connections;
    private readonly IChatify _chatify;

    public ChatifyChatSignalREventSubscriber(IHubContext<THub> hub, IChatifyHubConnections connections, IChatify chatify)
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
            var message = new ChatMessageDto(
                x.MessageId,
                x.Chat.Id,
                x.Body.Text,
                x.SentAt,
                x.Body.Files,
                sentBy: x.AuthorId == AccountContext.System.Id
                    ? AccountDto.System()
                    : new AccountDto(accountById[x.AuthorId]));

            foreach (var member in x.Chat.Members)
            {
                var chatListItem = new ChatListItemDto(
                    x.Chat.Id,
                    x.Chat.Name,
                    message,
                    totalUnreadCount: member == x.ReadForId ? 0 : 1,
                    active: true,
                    lastReadMessageId: member == x.ReadForId ? x.MessageId : null);

                await _hub.Clients.User(member).SendCoreAsync(
                    "chatify://chat-created",
                    [chatListItem]);
            }
        }
    }

    public async Task InvokeAsync(IEnumerable<ChatMessageSentEvent> events)
    {
        events = events.ToArray();
        var chatMembersByChatId = await _chatify.ActiveChatMemberIdByChatIdAsync(events.Select(x => x.ChatId));
        var accountById = await _chatify.AccountByIdAsync(events.SelectMany(x => chatMembersByChatId[x.ChatId]));

        foreach (var sentEvent in events)
        {
            foreach (var recipient in chatMembersByChatId[sentEvent.ChatId])
            {
                var message = new ChatMessageDto(sentEvent.Id, sentEvent.ChatId, sentEvent.Body.Text, sentEvent.SentAt,
                    sentEvent.Body.Files,
                    sentBy: sentEvent.AuthorId == AccountContext.System.Id
                        ? AccountDto.System()
                        : new AccountDto(accountById[sentEvent.AuthorId]));

                await _hub.Clients.User(recipient).SendCoreAsync(
                    "chatify://chat-new-message",
                    [message]);
            }
        }
    }

    public async Task InvokeAsync(IEnumerable<ChatMessageReadEvent> events)
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
                "chatify://chat-message-read",
                [signals]);
        }
    }

    public async Task InvokeAsync(IEnumerable<ChatMemberAddedEvent> events)
    {
        events = events.ToArray();
        var accountById = await _chatify.AccountByIdAsync(ids: events.Select(x => x.AccountId).Distinct());
        var members = await _chatify.ChatMembersAsync(events.Select(x => x.ChatId).Distinct(), active: true);
        var chats = await _chatify.ChatByIdAsync(chatIds: events.Select(x => x.ChatId));
        var chatById = chats.ToDictionary(x => x.Id);

        foreach (var x in events)
        {
            var chatMembers = members[x.ChatId].Select(m => m.AccountId).Concat(new[] { x.AccountId }).Distinct();

            foreach (var member in chatMembers)
            {
                if (!_connections.Connected(member))
                    continue;

                var message = new ChatMessageDto(x.ChatMessage.Id, x.ChatMessage.ChatId, x.ChatMessage.Body.Text, x.ChatMessage.SentAt,
                    x.ChatMessage.Body.Files, AccountDto.System());

                await _hub.Clients.User(member).SendCoreAsync(
                    "chatify://chat-member-added",
                    [x.ChatId, chatById[x.ChatId].Name, new AccountDto(accountById[x.AccountId]), message, x.WithHistory, member == x.ReadForId]);
            }
        }
    }

    public async Task InvokeAsync(IEnumerable<ChatMemberRemovedEvent> events)
    {
        events = events.ToArray();
        var members = await _chatify.ChatMembersAsync(events.Select(x => x.ChatId).Distinct(), active: true);

        foreach (var x in events)
        {
            var chatMembers = members[x.ChatId].Select(m => m.AccountId).Concat(new[] { x.AccountId }).Distinct();


            foreach (var member in chatMembers)
            {
                if (!_connections.Connected(member))
                    continue;

                var message = new ChatMessageDto(x.ChatMessage.Id, x.ChatMessage.ChatId, x.ChatMessage.Body.Text, x.ChatMessage.SentAt,
                    x.ChatMessage.Body.Files, AccountDto.System());

                await _hub.Clients.User(member).SendCoreAsync(
                    "chatify://chat-member-removed",
                    [x.ChatId, x.AccountId, x.DeleteHistory, message, member == x.ReadForId]);
            }
        }
    }

    public async Task InvokeAsync(IEnumerable<ChatNameChangedEvent> events)
    {
        events = events.ToArray();
        var members = await _chatify.ChatMembersAsync(events.Select(x => x.ChatId).Distinct(), active: true);

        foreach (var x in events)
        {
            var chatMembers = members[x.ChatId].Select(m => m.AccountId).Distinct().ToArray();

            foreach (var member in chatMembers)
            {
                if (!_connections.Connected(member))
                    continue;

                var message = new ChatMessageDto(x.ChatMessage.Id, x.ChatMessage.ChatId, x.ChatMessage.Body.Text, x.ChatMessage.SentAt,
                    x.ChatMessage.Body.Files, AccountDto.System());

                await _hub.Clients.User(member).SendCoreAsync(
                    "chatify://chat-name-changed",
                    [x.ChatId, x.NewName, message, member == x.ReadForId]);
            }
        }
    }
}
