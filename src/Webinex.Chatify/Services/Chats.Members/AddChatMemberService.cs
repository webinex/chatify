using System.Data;
using LinqToDB;
using Webinex.Chatify.Abstractions;
using Webinex.Chatify.Abstractions.Events;
using Webinex.Chatify.Common;
using Webinex.Chatify.DataAccess;
using Webinex.Chatify.Rows.Chats;
using Webinex.Chatify.Services.Chats.Caches;
using Webinex.Chatify.Services.Chats.Messages;
using Webinex.Chatify.Services.Common.Caches;

namespace Webinex.Chatify.Services.Chats.Members;

internal interface IAddChatMemberService
{
    Task AddRangeAsync(IEnumerable<AddChatMemberArgs> args);
}

internal class AddChatMemberService : IAddChatMemberService
{
    private readonly IEventService _eventService;
    private readonly IEntityCache<ChatMembersCacheEntry> _chatMemberCache;
    private readonly IChatifyDataConnectionFactory _connectionFactory;

    public AddChatMemberService(
        IEventService eventService,
        IEntityCache<ChatMembersCacheEntry> chatMemberCache,
        IChatifyDataConnectionFactory connectionFactory)
    {
        _eventService = eventService;
        _chatMemberCache = chatMemberCache;
        _connectionFactory = connectionFactory;
    }

    public async Task AddRangeAsync(IEnumerable<AddChatMemberArgs> args)
    {
        args = args.ToArray();

        foreach (var arg in args)
        {
            if (arg.WithHistory) await AddWithHistoryAsync(arg);
            else await AddWithoutHistoryAsync(arg);
        }
    }

    private async Task AddWithHistoryAsync(AddChatMemberArgs args)
    {
        await using var connection = _connectionFactory.Create();
        await using var transaction = await connection.BeginTransactionAsync(IsolationLevel.ReadCommitted);

        var meta = await connection.GetMetaWithUpdLockAsync(args.ChatId);
        await connection.DeleteMembershipAsync(args.ChatId, args.AccountId);

        var messageRow = ChatMessageRow.NewMemberAdded(args.ChatId, meta.LastIndex + 1);
        await connection.InsertAsync(messageRow);
        meta.Increment(messageRow.Id);
        await connection.UpdateAsync(meta);

        var memberRow = ChatMemberRow.NewAdded(args.ChatId, args.AccountId, args.OnBehalfOf.Id,
            ChatMessageId.New(args.ChatId, 0).ToString());
        await connection.InsertAsync(memberRow);

        var newActivityRow = new ChatActivityRow(args.ChatId, args.AccountId, AccountContext.System.Id,
            messageRow.Id, true, ChatMessageId.New(args.ChatId, meta.LastIndex - 1).ToString());
        await connection.InsertOrReplaceAsync(newActivityRow);

        var readForId = !args.OnBehalfOf.IsSystem() ? args.OnBehalfOf.Id : null;
        await connection.NotifyMembersAsync(messageRow.Id, messageRow.AuthorId, readForId: readForId);

        await transaction.CommitAsync();

        await _chatMemberCache.InvalidateAsync(args.ChatId.ToString());

        _eventService.Push(new ChatMemberAddedEvent(args.ChatId, args.AccountId, ChatMessageMapper.Map(messageRow, null),
            true, readForId));
        await _eventService.FlushAsync();
    }

    private async Task AddWithoutHistoryAsync(AddChatMemberArgs args)
    {
        await using var connection = _connectionFactory.Create();
        await using var transaction = await connection.BeginTransactionAsync(IsolationLevel.ReadCommitted);

        var meta = await connection.GetMetaWithUpdLockAsync(args.ChatId);
        var readForId = !args.OnBehalfOf.IsSystem() ? args.OnBehalfOf.Id : null;
        var messageRow = ChatMessageRow.NewMemberAdded(args.ChatId, meta.LastIndex + 1);
        meta.Increment(messageRow.Id);
        await connection.SendMessageAsync(messageRow, readForId: readForId);
        await connection.UpdateAsync(meta);

        await connection.InsertAsync(ChatMemberRow.NewAdded(args.ChatId, args.AccountId, args.OnBehalfOf.Id,
            messageRow.Id));

        var newChatActivityRow = new ChatActivityRow(args.ChatId, args.AccountId, AccountContext.System.Id,
            messageRow.Id, true, null);

        await connection.InsertOrReplaceAsync(newChatActivityRow,
            (_, column, isInsert) =>
                isInsert || new[] { nameof(ChatActivityRow.LastMessageId), nameof(ChatActivityRow.Active) }
                    .Contains(column.MemberInfo.Name));

        await transaction.CommitAsync();

        await _chatMemberCache.InvalidateAsync(args.ChatId.ToString());

        _eventService.Push(
            new ChatMemberAddedEvent(args.ChatId, args.AccountId, ChatMessageMapper.Map(messageRow, null), false, readForId));
        await _eventService.FlushAsync();
    }
}
