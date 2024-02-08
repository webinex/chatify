using Microsoft.EntityFrameworkCore;
using Webinex.Chatify.Abstractions;
using Webinex.Chatify.Common;
using Webinex.Chatify.DataAccess;
using Webinex.Chatify.Rows;
using System.Data;
using System.Text.Json;
using Microsoft.Data.SqlClient;
using Webinex.Chatify.Abstractions.Events;

namespace Webinex.Chatify.Services.Messages;

internal interface ISendMessageService
{
    Task<Message[]> SendRangeAsync(IEnumerable<SendMessageArgs> argEnumerable);
}

internal class SendMessageService : ISendMessageService
{
    private readonly ChatifyDbContext _dbContext;
    private readonly IEventService _eventService;

    public SendMessageService(
        ChatifyDbContext dbContext,
        IEventService eventService)
    {
        _dbContext = dbContext;
        _eventService = eventService;
    }

    public async Task<Message[]> SendRangeAsync(IEnumerable<SendMessageArgs> argEnumerable)
    {
        var args = argEnumerable.ToArray();
        var result = new LinkedList<Message>();
        foreach (var arg in args)
        {
            result.AddLast(await SendAsync(arg));
        }

        return result.ToArray();
    }

    private async Task<Message> SendAsync(SendMessageArgs args)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted);
        var now = DateTimeOffset.UtcNow;

        var parameters = new List<SqlParameter>
        {
            new("chatId", args.ChatId),
            new("authorId", args.OnBehalfOf.Id),
            new("now", now),
            new("ticks", now.Ticks),
            new("messageText", args.Body.Text),
            new("files", JsonSerializer.Serialize(args.Body.Files, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            })),
        };

#pragma warning disable EF1002
        var result = (await _dbContext.Database.SqlQueryRaw<Result>(
#pragma warning restore EF1002
            $"""
             declare @index int;

             {ChatMetaRow.FormatSqlIncrement(("@chatId", "@index"))}
             declare @messageId char(65) = CONCAT(@chatId, '-', REPLACE(STR(@index, 9), SPACE(1), '0'), '-', @ticks);
             {MessageRow.FormatSqlInsert(("@messageId", "@chatId", "@messageText", "@authorId", "@now", "@index", "@files"))}

             update chatify.ChatActivities
                set LastMessageId = @messageId,
                    LastMessageFromId = @authorId,
                    LastMessageIndex = @index,
                    LastReadMessageId = case when AccountId = @authorId then @messageId else LastReadMessageId end,
                    LastReadMessageIndex = case when AccountId = @authorId then @index else LastReadMessageIndex end
                where ChatId = @chatId and Active = 1

             select @index as [Index], @messageId as MessageId;
             """,
            parameters.ToArray()).ToArrayAsync()).First();

        await transaction.CommitAsync();
        var row = new MessageRow(result.MessageId, args.ChatId, args.OnBehalfOf.Id, now, args.Body.Text,
            args.Body.Files);

        _eventService.Push(new MessageSentEvent(row.Id, row.ChatId, new MessageBody(row.Text, row.Files), row.AuthorId,
            row.SentAt, args.RequestId));
        await _eventService.FlushAsync();
        return MessageMapper.Map(row, null);
    }

    public class Result
    {
        public int Index { get; set; }
        public string MessageId { get; set; } = null!;
    }
}
