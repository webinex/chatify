using Microsoft.EntityFrameworkCore;
using Webinex.Chatify.Abstractions;
using Webinex.Chatify.Common;
using Webinex.Chatify.DataAccess;

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
        var deliveries =
            await _dbContext.Deliveries.Where(x => x.ToId == args.OnBehalfOf.Id && args.MessageIds.Contains(x.MessageId))
                .ToArrayAsync();

        foreach (var delivery in deliveries)
        {
            delivery.MarkRead(_eventService);
        }

        await _eventService.FlushAsync();
        await _dbContext.SaveChangesAsync();
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
