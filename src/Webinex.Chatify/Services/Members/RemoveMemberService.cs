using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Webinex.Chatify.Abstractions;
using Webinex.Chatify.Abstractions.Events;
using Webinex.Chatify.Common;
using Webinex.Chatify.DataAccess;
using Webinex.Chatify.Rows;
using Webinex.Chatify.Services.Caches;
using Webinex.Chatify.Services.Caches.Common;
using Webinex.Chatify.Services.Messages;
using File = Webinex.Chatify.Abstractions.File;

namespace Webinex.Chatify.Services.Members;

internal interface IRemoveMemberService
{
    Task RemoveRangeAsync(IEnumerable<RemoveMemberArgs> args);
}

internal class RemoveMemberService : IRemoveMemberService
{
    private readonly ChatifyDbContext _dbContext;
    private readonly IEventService _eventService;
    private readonly IEntityCache<ChatMembersCacheEntry> _chatMemberCache;
    private readonly IGetMemberService _getMemberService;

    public RemoveMemberService(
        ChatifyDbContext dbContext,
        IEventService eventService,
        IEntityCache<ChatMembersCacheEntry> chatMemberCache,
        IGetMemberService getMemberService)
    {
        _dbContext = dbContext;
        _eventService = eventService;
        _chatMemberCache = chatMemberCache;
        _getMemberService = getMemberService;
    }

    public async Task RemoveRangeAsync(IEnumerable<RemoveMemberArgs> args)
    {
        args = args.ToArray();
        foreach (var arg in args)
        {
            if (arg.DeleteHistory) await RemoveDeleteHistoryAsync(arg);
            else await RemoveKeepHistoryAsync(arg);
        }
    }

    private async Task RemoveDeleteHistoryAsync(RemoveMemberArgs args)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted);
        var now = DateTimeOffset.UtcNow;

        var parameters = new List<SqlParameter>
        {
            new("chatId", args.ChatId),
            new("memberId", args.AccountId),
            new("now", now),
            new("ticks", now.Ticks),
            new("messageText", "chatify://member-removed"),
            new("userId", args.OnBehalfOf.Id),
            new("systemId", AccountContext.System.Id),
        };

#pragma warning disable EF1002
        var result = (await _dbContext.Database.SqlQueryRaw<Result>(
#pragma warning restore EF1002
            $"""
             declare @index int;

             {ChatMetaRow.FormatSqlIncrement(("@chatId", "@index"))}
             declare @messageId char(65) = CONCAT(@chatId, '-', REPLACE(STR(@index, 9), SPACE(1), '0'), '-', @ticks);
             {MessageRow.FormatSqlInsert(("@messageId", "@chatId", "@messageText", "@systemId", "@now", "@index", null))}
             
             update chatify.ChatActivities
             set LastMessageId = @messageId, LastMessageIndex = @index, LastMessageFromId = @systemId
             where ChatId = @chatId and Active = 1
             
             delete from chatify.Members where ChatId = @chatId and AccountId = @memberId
             delete from chatify.ChatActivities where ChatId = @chatId and AccountId = @memberId

             select @index as [Index], @messageId as MessageId;
             """,
            parameters.ToArray()).ToArrayAsync()).First();

        await transaction.CommitAsync();
        await _chatMemberCache.InvalidateAsync(args.ChatId.ToString());

        var messageRow = new MessageRow(result.MessageId, args.ChatId, "chatify::system", now,
            "chatify://member-removed",
            Array.Empty<File>());

        _eventService.Push(new MemberRemovedEvent(args.ChatId, args.AccountId, true,
            MessageMapper.Map(messageRow, null)));
        await _eventService.FlushAsync();
    }

    private async Task RemoveKeepHistoryAsync(RemoveMemberArgs args)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted);
        var now = DateTimeOffset.UtcNow;

        var parameters = new List<SqlParameter>
        {
            new("chatId", args.ChatId),
            new("memberId", args.AccountId),
            new("now", now),
            new("ticks", now.Ticks),
            new("messageText", "chatify://member-removed"),
            new("userId", args.OnBehalfOf.Id),
            new("systemId", AccountContext.System.Id)
        };

#pragma warning disable EF1002
        var result = (await _dbContext.Database.SqlQueryRaw<Result>(
#pragma warning restore EF1002
            $"""
             declare @index int;

             {ChatMetaRow.FormatSqlIncrement(("@chatId", "@index"))}
             declare @messageId char(65) = CONCAT(@chatId, '-', REPLACE(STR(@index, 9), SPACE(1), '0'), '-', @ticks);
             {MessageRow.FormatSqlInsert(("@messageId", "@chatId", "@messageText", "@systemId", "@now", "@index", null))}
             
             update chatify.ChatActivities
                set
                    LastMessageId = @messageId,
                    LastMessageIndex = @index,
                    LastMessageFromId = @systemId,
                    Active = case when AccountId = @memberId then 0 else [Active] end
                where ChatId = @chatId and Active = 1
             
             update chatify.Members
                set LastMessageId = @messageId, LastMessageIndex = @index
                where ChatId = @chatId and AccountId = @memberId and LastMessageId is null

             select @index as [Index], @messageId as MessageId;
             """,
            parameters.ToArray()).ToArrayAsync()).First();

        await transaction.CommitAsync();
        await _chatMemberCache.InvalidateAsync(args.ChatId.ToString());

        var messageRow = new MessageRow(result.MessageId, args.ChatId, "chatify::system", now,
            "chatify://member-removed",
            Array.Empty<File>());

        _eventService.Push(new MemberRemovedEvent(args.ChatId, args.AccountId, false,
            MessageMapper.Map(messageRow, null)));
        await _eventService.FlushAsync();
    }

    private class Result
    {
        public int Index { get; set; }
        public string MessageId { get; set; } = null!;
    }
}
