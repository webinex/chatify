using Microsoft.EntityFrameworkCore;
using Webinex.Asky;
using Webinex.Chatify.Abstractions;
using Webinex.Chatify.DataAccess;
using Webinex.Chatify.Rows;

namespace Webinex.Chatify.Services.Chats;

internal interface IChatQueryService
{
    Task<Chat[]> QueryAsync(ChatQuery query);
}

internal class ChatQueryService : IChatQueryService
{
    private readonly ChatifyDbContext _dbContext;
    private readonly IAskyFieldMap<ChatActivityRow> _fieldMap;

    public ChatQueryService(
        ChatifyDbContext dbContext,
        IAskyFieldMap<ChatActivityRow> fieldMap)
    {
        _dbContext = dbContext;
        _fieldMap = fieldMap;
    }

    public async Task<Chat[]> QueryAsync(ChatQuery query)
    {
        var queryable = _dbContext.ChatActivities
            .AsQueryable()
            .Where(x => x.AccountId == query.OnBehalfOf.Id)
            .Include(x => x.Chat)
            .AsNoTrackingWithIdentityResolution();

        if (query.FilterRule != null)
            queryable = queryable.Where(_fieldMap, query.FilterRule);

        if (query.SortRule?.Any() == true)
            queryable = queryable.SortBy(_fieldMap, query.SortRule);

        if (query.Props.HasLastMessage())
            queryable = queryable.Include(x => x.Delivery!).ThenInclude(x => x.Message!);

        if (query.Props.ToLastMessageProps().HasFlag(Message.Props.Author))
            queryable = queryable.Include(x => x.Delivery!).ThenInclude(x => x.Message!).ThenInclude(x => x.Author);

        if (query.Props.ToLastMessageProps().HasFlag(Message.Props.Read))
            queryable = queryable.Include(x => x.Delivery!);

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
        var chatIds = activityRows.Select(x => x.ChatId).Distinct().ToArray();

        var result = await _dbContext.Deliveries
            .Where(x => chatIds.Contains(x.ChatId) && !x.Read && x.ToId == query.OnBehalfOf.Id)
            .GroupBy(x => new { x.ChatId, x.ToId })
            .Select(x => new { x.Key.ChatId, Count = x.Count() })
            .ToArrayAsync();

        return chatIds.ToDictionary(x => x, x => result.FirstOrDefault(g => g.ChatId == x)?.Count ?? 0);
    }
}
