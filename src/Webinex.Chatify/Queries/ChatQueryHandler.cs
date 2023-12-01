using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Webinex.Asky;
using Webinex.Chatify.Abstractions;
using Webinex.Chatify.DataAccess;
using Webinex.Chatify.Types;

namespace Webinex.Chatify.Queries;

internal class ChatQueryHandler
{
    private readonly ChatifyDbContext _dbContext;
    private readonly IAskyFieldMap<View> _chatQueryViewFieldMap;
    private readonly ChatQuery _query;

    public ChatQueryHandler(
        ChatifyDbContext dbContext,
        IAskyFieldMap<View> chatQueryViewFieldMap,
        ChatQuery query)
    {
        _dbContext = dbContext;
        _chatQueryViewFieldMap = chatQueryViewFieldMap;
        _query = query;
    }

    public async Task<ChatQueryResult> QueryAsync()
    {
        IQueryable<View> queryable = _dbContext.ChatActivities
            .Join(_dbContext.Chats, d => d.ChatId, c => c.Id,
                (lastDelivery, chat) => new { lastDelivery, chat })
            .Join(
                _dbContext.Deliveries,
                join => new
                {
                    ChatId = join.lastDelivery.ChatId,
                    MessageId = join.lastDelivery.LastMessageId,
                    ToId = join.lastDelivery.AccountId
                },
                delivery => new { delivery.ChatId, delivery.MessageId, delivery.ToId },
                (join, delivery) => new { join.lastDelivery, join.chat, delivery })
            .Join(
                _dbContext.Accounts,
                join => join.lastDelivery.LastMessageFromId,
                account => account.Id,
                (join, author) => new { join.lastDelivery, join.delivery, join.chat, author })
            .Join(
                _dbContext.Messages,
                join => join.delivery.MessageId,
                message => message.Id,
                (join, message) => new View
                {
                    Chat = join.chat,
                    ChatActivity = join.lastDelivery,
                    Message = message,
                    Author = join.author,
                    Delivery = join.delivery,
                });

        queryable = queryable.Where(x => x.ChatActivity.AccountId == _query.OnBehalfOf.Id);

        if (_query.FilterRule != null)
            queryable = queryable.Where(_chatQueryViewFieldMap, _query.FilterRule);

        if (_query.SortRule?.Any() == true)
            queryable = queryable.SortBy(_chatQueryViewFieldMap, _query.SortRule);

        if (_query.PagingRule != null)
            queryable = queryable.PageBy(_query.PagingRule);

        queryable = queryable.AsNoTrackingWithIdentityResolution();

        var result = _query.Prop.HasFlag(ChatQueryProp.LastMessage)
            ? await queryable
                .Select(x => new { x.Chat, LastDelivery = x.ChatActivity, x.Delivery, x.Message, x.Author })
                .ToArrayAsync()
            : (await queryable.Select(x => new
            {
                x.Chat,
                LastDelivery = default(ChatActivity?),
                Delivery = default(Delivery?),
                Message = default(Message?),
                Author = default(Account?),
            }).ToArrayAsync())!;

        var chatIds = result.Select(x => x.Chat.Id).Distinct().ToArray();

        IDictionary<Chat, int>? unreadCount = null;

        if (_query.Prop.HasFlag(ChatQueryProp.UnreadCount))
        {
            var unreadGroup = await _dbContext.Deliveries
                .Where(x => chatIds.Contains(x.ChatId) && !x.Read && x.ToId == _query.OnBehalfOf.Id)
                .GroupBy(x => new { x.ChatId, x.ToId })
                .Select(x => new { x.Key.ChatId, Count = x.Count() })
                .ToArrayAsync();

            unreadCount = result.ToDictionary(x => x.Chat,
                x => unreadGroup.FirstOrDefault(g => g.ChatId == x.Chat.Id)?.Count ?? 0);
        }

        return new ChatQueryResult(
            _query,
            result.Select(x => x.Chat).ToArray(),
            _query.Prop.HasFlag(ChatQueryProp.LastMessage)
                ? result.ToDictionary(x => x.Chat,
                    x => new ChatQueryResult.LastMessage(x.Message, x.Delivery, x.Author))
                : null,
            unreadCount);
    }

    internal class View
    {
        public Chat Chat { get; init; } = null!;
        public ChatActivity ChatActivity { get; init; } = null!;
        public Delivery Delivery { get; init; } = null!;
        public Message Message { get; init; } = null!;
        public Account Author { get; init; } = null!;
    }

    internal class FieldMap : IAskyFieldMap<View>
    {
        public Expression<Func<View, object>>? this[string fieldId] => fieldId switch
        {
            "id" => x => x.Chat.Id,
            "name" => x => x.Chat.Name,
            "createdAt" => x => x.Chat.CreatedAt,
            "createdById" => x => x.Chat.CreatedById,
            "read" => x => x.Delivery.Read,
            _ => null,
        };
    }
}