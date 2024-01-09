using Webinex.Chatify.Abstractions;
using Webinex.Chatify.Common;
using Webinex.Chatify.DataAccess;
using Webinex.Chatify.Rows;

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

        var rows = args.Select(x => MessageRow.NewSent(
            _eventService,
            x.ChatId,
            x.OnBehalfOf.Id,
            x.Body,
            requestId: x.RequestId)).ToArray();

        await _dbContext.Messages.AddRangeAsync(rows);
        await _eventService.FlushAsync();
        await _dbContext.SaveChangesAsync();
        return rows.Select(x => MessageMapper.Map(x)).ToArray();
    }
}
