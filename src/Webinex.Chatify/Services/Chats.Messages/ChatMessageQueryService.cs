using LinqToDB;
using Webinex.Asky;
using Webinex.Chatify.Abstractions;
using Webinex.Chatify.DataAccess;
using Webinex.Chatify.Rows.Chats;

namespace Webinex.Chatify.Services.Chats.Messages;

internal interface IChatMessageQueryService
{
    Task<ChatMessage[]> QueryAsync(ChatMessageQuery query);
}

internal class ChatMessageQueryService : IChatMessageQueryService
{
    private readonly IChatifyDataConnectionFactory _dataConnectionFactory;
    private readonly IAskyFieldMap<ChatMessageRow> _messageRowFieldMap;

    public ChatMessageQueryService(
        IChatifyDataConnectionFactory dataConnectionFactory,
        IAskyFieldMap<ChatMessageRow> messageRowFieldMap)
    {
        _dataConnectionFactory = dataConnectionFactory;
        _messageRowFieldMap = messageRowFieldMap;
    }

    public async Task<ChatMessage[]> QueryAsync(ChatMessageQuery query)
    {
        await using var connection = _dataConnectionFactory.Create();
        
        var queryable = connection.MessageRows
                .Where(ChatMessageRow.In(connection.MemberRows.AsQueryable().Where(x => x.AccountId == query.OnBehalfOf.Id)));

        if (query.Props.HasFlag(ChatMessage.Props.Author))
            queryable = queryable.LoadWith(x => x.Author);

        if (query.FilterRule != null)
            queryable = queryable.Where(_messageRowFieldMap, query.FilterRule);

        if (query.SortRule?.Any() == true)
            queryable = queryable.SortBy(_messageRowFieldMap, query.SortRule);

        if (query.PagingRule != null)
            queryable = queryable.PageBy(query.PagingRule);

        return query.Props.HasDeliveryProps()
            ? await ToDeliveryMessagesAsync(connection, query, queryable)
            : await ToMessagesAsync(query, queryable);
    }

    private async Task<ChatMessage[]> ToDeliveryMessagesAsync(ChatifyDataConnection connection, ChatMessageQuery query, IQueryable<ChatMessageRow> queryable)
    {
        var result = await queryable.Join(connection.ChatActivityRows,
            x => new { x.ChatId, AccountId = query.OnBehalfOf.Id }, a => new { a.ChatId, a.AccountId },
            (message, activity) => new { Message = message, activity.LastReadMessageId }).ToArrayAsync();

        return result.Select(x => ChatMessageMapper.Map(
            x.Message,
            read: x.LastReadMessageId != null && x.Message.Id.CompareTo(x.LastReadMessageId) < 0,
            query.Props)).ToArray();
    }

    private async Task<ChatMessage[]> ToMessagesAsync(ChatMessageQuery query, IQueryable<ChatMessageRow> queryable)
    {
        var result = await queryable.ToArrayAsync();
        return result.Select(x => ChatMessageMapper.Map(x, null, query.Props)).ToArray();
    }
}
