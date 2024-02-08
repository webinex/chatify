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
            queryable = queryable.Include(x => x.LastMessage);

        if (query.Props.ToLastMessageProps().HasFlag(Message.Props.Author))
            queryable = queryable.Include(x => x.LastMessage!).ThenInclude(x => x.Author);

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

        var queryable = from member in _dbContext.Members
            join activity in _dbContext.ChatActivities on new { member.ChatId, member.AccountId } equals
                new { activity.ChatId, activity.AccountId }
            where member.AccountId == query.OnBehalfOf.Id && chatIds.Contains(member.ChatId) && (member.LastMessageIndex == null || activity.LastReadMessageIndex == null || member.LastMessageIndex > activity.LastReadMessageIndex)
            select new
            {
                member.ChatId,
                member.FirstMessageIndex,
                member.LastMessageIndex,
            };

        var joinResult = await queryable.ToArrayAsync();
        
        return activityRows.ToDictionary(x => x.ChatId, a =>
        {
            var members = joinResult.Where(m => m.ChatId == a.ChatId);
            return members.Sum(x =>
                x.LastMessageIndex.HasValue
                    ? x.LastMessageIndex - (a.LastReadMessageIndex ?? -1)
                    : a.LastMessageIndex - (a.LastReadMessageIndex ?? -1)) ?? 0;
        });
    }
}
