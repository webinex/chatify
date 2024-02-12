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

internal interface IAddMemberService
{
    Task AddRangeAsync(IEnumerable<AddMemberArgs> args);
}

internal class AddMemberService : IAddMemberService
{
    private readonly ChatifyDbContext _dbContext;
    private readonly IEventService _eventService;
    private readonly IEntityCache<ChatMembersCacheEntry> _chatMemberCache;

    public AddMemberService(
        ChatifyDbContext dbContext,
        IEventService eventService,
        IEntityCache<ChatMembersCacheEntry> chatMemberCache)
    {
        _dbContext = dbContext;
        _eventService = eventService;
        _chatMemberCache = chatMemberCache;
    }

    public async Task AddRangeAsync(IEnumerable<AddMemberArgs> args)
    {
        args = args.ToArray();

        foreach (var arg in args)
        {
            if (arg.WithHistory) await AddWithHistoryAsync(arg);
            else await AddWithoutHistoryAsync(arg);
        }
    }

    private async Task AddWithHistoryAsync(AddMemberArgs args)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted);
        var now = DateTimeOffset.UtcNow;

        var parameters = new List<SqlParameter>
        {
            new("chatId", args.ChatId),
            new("memberId", args.AccountId),
            new("now", now),
            new("ticks", now.Ticks),
            new("messageText", "chatify://member-added"),
            new("userId", args.OnBehalfOf.Id),
            new("systemId", AccountContext.System.Id),
        };

#pragma warning disable EF1002
        var result = (await _dbContext.Database.SqlQueryRaw<Result>(
#pragma warning restore EF1002
            $"""
             declare @index int;
             declare @firstMessageId char(65);
             declare @lastMessageId char(65);
             declare @lastMessageIndex int;

             select @firstMessageId = MIN(Id),
                    @lastMessageId = MAX(Id),
                    @lastMessageIndex = MAX([Index])
             from chatify.Messages
             where ChatId = @chatId
             group by ChatId

             {MemberRow.FormatSqlDelete(("@chatId", "@memberId"))}
             {ChatMetaRow.FormatSqlIncrement(("@chatId", "@index"))}
             declare @messageId char(65) = CONCAT(@chatId, '-', REPLACE(STR(@index, 9), SPACE(1), '0'), '-', @ticks);
             {MessageRow.FormatSqlInsert(("@messageId", "@chatId", "@messageText", "@systemId", "@now", "@index", null))}
             {MemberRow.FormatSqlInsert(("@chatId", "@memberId", "@userId", "@now", "@firstMessageId", "0"))}

             delete chatify.ChatActivities where ChatId = @chatId and AccountId = @memberId

             update chatify.ChatActivities
             set LastMessageId = @messageId, LastMessageIndex = @index, LastMessageFromId = @systemId
             where ChatId = @chatId and Active = 1

             insert into chatify.ChatActivities (
                 ChatId,
                 AccountId,
                 LastMessageFromId,
                 LastMessageId,
                 LastMessageIndex,
                 LastReadMessageId,
                 LastReadMessageIndex,
                 Active
             )
             values (
                 @chatId,
                 @memberId,
                 @systemId,
                 @messageId,
                 @index,
                 @lastMessageId,
                 @lastMessageIndex,
                 1
             )

             select @index as [Index], @messageId as MessageId;
             """,
            parameters.ToArray()).ToArrayAsync()).First();

        await transaction.CommitAsync();

        await _chatMemberCache.InvalidateAsync(args.ChatId.ToString());

        var messageRow = new MessageRow(result.MessageId, args.ChatId, "chatify::system", now, "chatify://member-added",
            Array.Empty<File>());

        _eventService.Push(new MemberAddedEvent(args.ChatId, args.AccountId, MessageMapper.Map(messageRow, null),
            true));
        await _eventService.FlushAsync();
    }

    private async Task AddWithoutHistoryAsync(AddMemberArgs args)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted);
        var now = DateTimeOffset.UtcNow;

        var parameters = new List<SqlParameter>
        {
            new("chatId", args.ChatId),
            new("memberId", args.AccountId),
            new("now", now),
            new("ticks", now.Ticks),
            new("messageText", "chatify://member-added"),
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
             {MemberRow.FormatSqlInsert(("@chatId", "@memberId", "@userId", "@now", "@messageId", "@index"))}

             if exists (select 1 from chatify.ChatActivities where ChatId = @chatId and AccountId = @memberId)
             begin
                 update chatify.ChatActivities
                 set LastMessageFromId = @systemId,
                     LastMessageId = @messageId,
                     LastMessageIndex = @index,
                     Active = 1
                 where ChatId = @chatId and AccountId = @memberId
             end
             else
             begin
                insert into chatify.ChatActivities (
                    ChatId,
                    AccountId,
                    LastMessageFromId,
                    LastMessageId,
                    LastMessageIndex,
                    Active
                )
                values (
                    @chatId,
                    @memberId,
                    @systemId,
                    @messageId,
                    @index,
                    1
                )
                
                update chatify.ChatActivities
                    set LastMessageId = @messageId, LastMessageIndex = @index, LastMessageFromId = @systemId
                    where ChatId = @chatId and Active = 1
             end

             select @index as [Index], @messageId as MessageId;
             """,
            parameters.ToArray()).ToArrayAsync()).First();

        await transaction.CommitAsync();

        await _chatMemberCache.InvalidateAsync(args.ChatId.ToString());

        var messageRow = new MessageRow(result.MessageId, args.ChatId, "chatify::system", now, "chatify://member-added",
            Array.Empty<File>());

        _eventService.Push(
            new MemberAddedEvent(args.ChatId, args.AccountId, MessageMapper.Map(messageRow, null), false));
        await _eventService.FlushAsync();
    }

    private class Result
    {
        public int Index { get; set; }
        public string MessageId { get; set; } = null!;
    }
}
