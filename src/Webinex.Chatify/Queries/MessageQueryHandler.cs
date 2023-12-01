using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Webinex.Asky;
using Webinex.Chatify.Abstractions;
using Webinex.Chatify.DataAccess;
using Webinex.Chatify.Types;

namespace Webinex.Chatify.Queries;

internal class MessageQueryHandler
{
    private readonly ChatifyDbContext _dbContext;
    private readonly IAskyFieldMap<View> _messageQueryViewFieldMap;
    private readonly MessageQuery _query;

    public MessageQueryHandler(
        ChatifyDbContext dbContext,
        IAskyFieldMap<View> messageQueryViewFieldMap,
        MessageQuery query)
    {
        _dbContext = dbContext;
        _messageQueryViewFieldMap = messageQueryViewFieldMap;
        _query = query;
    }

    public async Task<MessageQueryResult> QueryAsync()
    {
        var queryable = _dbContext.Deliveries.AsQueryable()
            .Join(
                _dbContext.Messages,
                delivery => delivery.MessageId,
                message => message.Id,
                (delivery, message) => new { delivery, message })
            .Join(
                _dbContext.Accounts,
                join => join.delivery.FromId,
                account => account.Id,
                (join, author) => new { join.delivery, join.message, author })
            .Select(join => new View
            {
                Message = join.message,
                Author = join.author,
                Delivery = join.delivery,
            });

        queryable = queryable.Where(x => x.Delivery.ToId == _query.OnBehalfOf.Id);

        if (_query.FilterRule != null)
            queryable = queryable.Where(_messageQueryViewFieldMap, _query.FilterRule);

        if (_query.SortRule?.Any() == true)
            queryable = queryable.SortBy(_messageQueryViewFieldMap, _query.SortRule);

        if (_query.PagingRule != null)
            queryable = queryable.PageBy(_query.PagingRule);

        var result = await queryable.ToArrayAsync();
        var entries = result.Select(x => new MessageQueryResult.Entry(x.Message, x.Delivery, x.Author)).ToArray();
        return new MessageQueryResult(_query, entries);
    }

    internal class View
    {
        public Message Message { get; init; } = null!;
        public Delivery Delivery { get; init; } = null!;
        public Account Author { get; init; } = null!;
    }

    internal class FieldMap : IAskyFieldMap<View>
    {
        public Expression<Func<View, object>>? this[string fieldId] => fieldId switch
        {
            "chatId" => x => x.Delivery.ChatId,
            "sentAt" => x => x.Message.SentAt,
            _ => null,
        };
    }
}