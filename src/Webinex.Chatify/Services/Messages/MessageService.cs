using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Webinex.Chatify.Abstractions;
using Webinex.Chatify.Abstractions.Events;
using Webinex.Chatify.Common;
using Webinex.Chatify.DataAccess;
using Webinex.Chatify.Rows;

namespace Webinex.Chatify.Services.Messages;

internal interface IMessageService
{
    Task ReadAsync(ReadArgs args);
    Task<Message[]> QueryAsync(MessageQuery query);
    Task<Message[]> SendRangeAsync(IEnumerable<SendMessageArgs> args);
}

internal class MessageService : IMessageService
{
    private readonly ChatifyDbContext _dbContext;
    private readonly IEventService _eventService;
    private readonly ISendMessageService _sendMessageService;
    private readonly IMessageQueryService _messageQueryService;

    public MessageService(
        ChatifyDbContext dbContext,
        IEventService eventService,
        ISendMessageService sendMessageService,
        IMessageQueryService messageQueryService)
    {
        _dbContext = dbContext;
        _eventService = eventService;
        _sendMessageService = sendMessageService;
        _messageQueryService = messageQueryService;
    }

    public async Task ReadAsync(ReadArgs args)
    {
        var messageId = MessageId.Parse(args.Id);

        await using var transaction = await _dbContext.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted);

        var parameters = new List<SqlParameter>
        {
            new("chatId", messageId.ChatId),
            new("accountId", args.OnBehalfOf.Id),
            new("lastReadMessageId", messageId.ToString()),
            new("lastReadMessageIndex", messageId.Index),
        };
        
        var readCount = (await _dbContext.Database.SqlQueryRaw<int>(
            """
                declare @previousLastReadMessageId char(65);
            
                update chatify.ChatActivities with (rowlock, updlock, holdlock)
                set
                    LastReadMessageId = case when LastReadMessageId < @lastReadMessageId or LastReadMessageId is null then @lastReadMessageId else LastReadMessageId end,
                    LastReadMessageIndex = case when LastReadMessageIndex < @lastReadMessageIndex or LastReadMessageIndex is null then @lastReadMessageIndex else LastReadMessageIndex end,
                    @previousLastReadMessageId = LastReadMessageId
                where ChatId = @chatId and AccountId = @accountId
                
                select count(*) from chatify.Messages message
                inner join chatify.Members member on message.ChatId = member.ChatId and member.AccountId = @accountId and member.FirstMessageId <= message.Id and (member.LastMessageId is null or member.LastMessageId >= message.Id)
                where message.ChatId = @chatId and message.Id <= @lastReadMessageId and (@previousLastReadMessageId is null or message.Id > @previousLastReadMessageId)
            """, parameters.ToArray()).ToArrayAsync()).First();

        await transaction.CommitAsync();
        _eventService.Push(new ReadEvent(messageId.ChatId, args.OnBehalfOf.Id, messageId.ToString(), readCount));
        await _eventService.FlushAsync();
    }

    public async Task<Message[]> QueryAsync(MessageQuery query)
    {
        return await _messageQueryService.QueryAsync(query);
    }

    public async Task<Message[]> SendRangeAsync(IEnumerable<SendMessageArgs> args)
    {
        return await _sendMessageService.SendRangeAsync(args);
    }
}
