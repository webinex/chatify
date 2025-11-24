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

internal interface IRemoveChatMemberService
{
    Task RemoveRangeAsync(IEnumerable<RemoveChatMemberArgs> args);
}

internal class RemoveChatMemberService : IRemoveChatMemberService
{
    private readonly IEventService _eventService;
    private readonly IEntityCache<ChatMembersCacheEntry> _chatMemberCache;
    private readonly IChatifyDataConnectionFactory _dataConnectionFactory;

    public RemoveChatMemberService(
        IEventService eventService,
        IEntityCache<ChatMembersCacheEntry> chatMemberCache,
        IChatifyDataConnectionFactory dataConnectionFactory)
    {
        _eventService = eventService;
        _chatMemberCache = chatMemberCache;
        _dataConnectionFactory = dataConnectionFactory;
    }

    public async Task RemoveRangeAsync(IEnumerable<RemoveChatMemberArgs> args)
    {
        args = args.ToArray();
        foreach (var arg in args)
        {
            await RemoveAsync(arg);
        }
    }

    private async Task RemoveAsync(RemoveChatMemberArgs args)
    {
        
        await using var connection = _dataConnectionFactory.Create();
        await using var transaction = await connection.BeginTransactionAsync(IsolationLevel.ReadCommitted);
        
        var meta = await connection.GetMetaWithUpdLockAsync(args.ChatId);
        var messageRow = ChatMessageRow.NewMemberRemoved(args.ChatId, meta.LastIndex + 1);
        meta.Increment(messageRow.Id);
        var readForId = args.OnBehalfOf.IsSystem() ? null : args.OnBehalfOf.Id;
        await connection.SendMessageAsync(messageRow, except: args.DeleteHistory ? new[] { args.AccountId } : null, readForId);
        await connection.UpdateAsync(meta);

        if (args.DeleteHistory)
        {
            await connection.DeleteMembershipAsync(args.ChatId, args.AccountId);
            await connection.DeleteChatActivityAsync(args.ChatId, args.AccountId);
        }
        else
        {
            await connection.DeactivateChatActivityAsync(args.ChatId, args.AccountId);
            await connection.DeactivateMembershipAsync(args.ChatId, args.AccountId, messageRow.Id);
        }

        await transaction.CommitAsync();
        await _chatMemberCache.InvalidateAsync(args.ChatId.ToString());

        _eventService.Push(new ChatMemberRemovedEvent(args.ChatId, args.AccountId, args.DeleteHistory,
            ChatMessageMapper.Map(messageRow, null), readForId));
        await _eventService.FlushAsync();
    }
}
