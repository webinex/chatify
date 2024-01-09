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
    private readonly IAskyFieldMap<DeliveryRow> _deliveryRowFieldMap;

    public MessageQueryService(
        ChatifyDbContext dbContext,
        IAskyFieldMap<DeliveryRow> deliveryRowFieldMap)
    {
        _dbContext = dbContext;
        _deliveryRowFieldMap = deliveryRowFieldMap;
    }

    public async Task<Message[]> QueryAsync(MessageQuery query)
    {
        var queryable = _dbContext.Deliveries.AsQueryable()
            .Where(x => x.ToId == query.OnBehalfOf.Id)
            .Include(x => x.Message)
            .AsNoTrackingWithIdentityResolution();

        if (query.Props.HasFlag(Message.Props.Author))
            queryable = queryable.Include(x => x.Message!.Author);

        if (query.FilterRule != null)
            queryable = queryable.Where(_deliveryRowFieldMap, query.FilterRule);

        if (query.SortRule?.Any() == true)
            queryable = queryable.SortBy(_deliveryRowFieldMap, query.SortRule);

        if (query.PagingRule != null)
            queryable = queryable.PageBy(query.PagingRule);

        return query.Props.HasDeliveryProps()
            ? await ToDeliveryMessagesAsync(query, queryable)
            : await ToMessagesAsync(query, queryable);
    }

    private async Task<Message[]> ToDeliveryMessagesAsync(MessageQuery query, IQueryable<DeliveryRow> queryable)
    {
        var result = await queryable.ToArrayAsync();
        return result.Select(x => MessageMapper.Map(x.Message!, x, query.Props)).ToArray();
    }

    private async Task<Message[]> ToMessagesAsync(MessageQuery query, IQueryable<DeliveryRow> queryable)
    {
        var result = await queryable.Select(x => x.Message!).ToArrayAsync();
        return result.Select(x => MessageMapper.Map(x, null, query.Props)).ToArray();
    }
}
