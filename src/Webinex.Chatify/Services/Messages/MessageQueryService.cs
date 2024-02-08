using Microsoft.EntityFrameworkCore;
using Webinex.Asky;
using Webinex.Chatify.Abstractions;
using Webinex.Chatify.DataAccess;
using Webinex.Chatify.Rows;

namespace Webinex.Chatify.Services.Messages;

internal interface IMessageQueryService
{
    Task<Message[]> QueryAsync(MessageQuery query);
}

internal class MessageQueryService : IMessageQueryService
{
    private readonly ChatifyDbContext _dbContext;
    private readonly IAskyFieldMap<MessageRow> _messageRowFieldMap;

    public MessageQueryService(
        ChatifyDbContext dbContext,
        IAskyFieldMap<MessageRow> messageRowFieldMap)
    {
        _dbContext = dbContext;
        _messageRowFieldMap = messageRowFieldMap;
    }

    public async Task<Message[]> QueryAsync(MessageQuery query)
    {
        var queryable = _dbContext.Messages
                .Where(MessageRow.In(_dbContext.Members.AsQueryable().Where(x => x.AccountId == query.OnBehalfOf.Id)))
                .AsNoTracking();

        if (query.Props.HasFlag(Message.Props.Author))
            queryable = queryable.Include(x => x.Author);

        if (query.FilterRule != null)
            queryable = queryable.Where(_messageRowFieldMap, query.FilterRule);

        if (query.SortRule?.Any() == true)
            queryable = queryable.SortBy(_messageRowFieldMap, query.SortRule);

        if (query.PagingRule != null)
            queryable = queryable.PageBy(query.PagingRule);

        return query.Props.HasDeliveryProps()
            ? await ToDeliveryMessagesAsync(query, queryable)
            : await ToMessagesAsync(query, queryable);
    }

    private async Task<Message[]> ToDeliveryMessagesAsync(MessageQuery query, IQueryable<MessageRow> queryable)
    {
        var result = await queryable.Join(_dbContext.ChatActivities,
            x => new { x.ChatId, AccountId = query.OnBehalfOf.Id }, a => new { a.ChatId, a.AccountId },
            (message, activity) => new { Message = message, activity.LastReadMessageIndex }).ToArrayAsync();

        return result.Select(x => MessageMapper.Map(
            x.Message,
            read: x.LastReadMessageIndex != null && x.Message.Index < x.LastReadMessageIndex,
            query.Props)).ToArray();
    }

    private async Task<Message[]> ToMessagesAsync(MessageQuery query, IQueryable<MessageRow> queryable)
    {
        var result = await queryable.ToArrayAsync();
        return result.Select(x => MessageMapper.Map(x, null, query.Props)).ToArray();
    }
}
