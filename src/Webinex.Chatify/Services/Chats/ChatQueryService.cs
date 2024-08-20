using LinqToDB;
using Webinex.Asky;
using Webinex.Chatify.Abstractions;
using Webinex.Chatify.DataAccess;
using Webinex.Chatify.Rows;
using Webinex.Chatify.Rows.Chats;

namespace Webinex.Chatify.Services.Chats;

internal interface IChatQueryService
{
    Task<Chat[]> QueryAsync(ChatQuery query);
}

internal class ChatQueryService : IChatQueryService
{
    private readonly IChatifyDataConnectionFactory _dataConnectionFactory;
    private readonly IAskyFieldMap<ChatActivityRow> _fieldMap;

    public ChatQueryService(
        IAskyFieldMap<ChatActivityRow> fieldMap,
        IChatifyDataConnectionFactory dataConnectionFactory)
    {
        _fieldMap = fieldMap;
        _dataConnectionFactory = dataConnectionFactory;
    }

    public async Task<Chat[]> QueryAsync(ChatQuery query)
    {
        await using var connection = _dataConnectionFactory.Create();

        var queryable = connection.ChatActivityRows
            .AsQueryable()
            .Where(x => x.AccountId == query.OnBehalfOf.Id);

        queryable = queryable.LoadWith(x => x.Chat);

        if (query.FilterRule != null)
            queryable = queryable.Where(_fieldMap, query.FilterRule);

        if (query.SortRule?.Any() == true)
            queryable = queryable.SortBy(_fieldMap, query.SortRule);

        if (query.Props.HasLastMessage())
            queryable = queryable.LoadWith(x => x.LastChatMessage);

        if (query.Props.ToLastMessageProps().HasFlag(ChatMessage.Props.Author))
            queryable = queryable.LoadWith(x => x.LastChatMessage).ThenLoad(x => x.Author);

        var activityRows = await queryable.ToArrayAsync();

        var totalUnreadCountByChatId = query.Props.HasFlag(Chat.Props.TotalUnreadCount)
            ? await TotalUnreadCountAsync(query, activityRows)
            : null;

        return activityRows.Select(x => ChatMapper.Map(x.Chat!, x, query.Props, totalUnreadCountByChatId)).ToArray();
    }

    private async Task<IReadOnlyDictionary<Guid, int>> TotalUnreadCountAsync(
        ChatQuery query,
        ChatActivityRow[] activityRows)
    {
        await using var connection = _dataConnectionFactory.Create();
        var chatIds = activityRows.Select(x => x.ChatId).Distinct().ToArray();
        var memberships
            = connection.MemberRows.Where(x => chatIds.Contains(x.ChatId) && x.AccountId == query.OnBehalfOf.Id);

        return activityRows.ToDictionary(x => x.ChatId,
            a => ChatUnreadCountUtil.Count(a, memberships.Where(x => x.ChatId == a.ChatId).ToArray()));
    }
}
