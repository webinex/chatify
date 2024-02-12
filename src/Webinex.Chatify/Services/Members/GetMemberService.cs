using Microsoft.EntityFrameworkCore;
using Webinex.Chatify.Abstractions;
using Webinex.Chatify.DataAccess;
using Webinex.Chatify.Services.Caches;
using Webinex.Chatify.Services.Caches.Common;

namespace Webinex.Chatify.Services.Members;

internal interface IGetMemberService
{
    Task<IReadOnlyDictionary<Guid, IReadOnlyCollection<Member>>> ByChatsAsync(IEnumerable<Guid> chatIds, bool? active = null);
    Task<IReadOnlyDictionary<Guid, string[]>> ActiveIdByChatIdAsync(IEnumerable<Guid> chatIds);
}

internal class GetMemberService : IGetMemberService
{
    private readonly IEntityCache<ChatMembersCacheEntry> _chatMemberCache;
    private readonly ChatifyDbContext _dbContext;

    public GetMemberService(IEntityCache<ChatMembersCacheEntry> chatMemberCache, ChatifyDbContext dbContext)
    {
        _chatMemberCache = chatMemberCache;
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyDictionary<Guid, IReadOnlyCollection<Member>>> ByChatsAsync(IEnumerable<Guid> chatIds, bool? active = null)
    {
        var queryable = _dbContext.Members.Where(x => chatIds.Contains(x.ChatId));
        if (active.HasValue)
            queryable = queryable.Where(x => (x.LastMessageId == null) == active);
        
        var rows = await queryable.ToArrayAsync();
        var rowByChatId = rows.ToLookup(x => x.ChatId);

        return chatIds.ToDictionary(id => id,
            id => (IReadOnlyCollection<Member>)rowByChatId[id].Select(x => x.ToAbstraction()).ToArray());
    }

    public async Task<IReadOnlyDictionary<Guid, string[]>> ActiveIdByChatIdAsync(IEnumerable<Guid> chatIds)
    {
        var keys = chatIds.Select(x => x.ToString()).ToArray();
        var entries = await _chatMemberCache.GetOrCreateAsync(
            keys,
            GetCacheAsync);
        return entries.ToDictionary(x => x.Value.ChatId, x => x.Value.Active.ToArray());
    }

    private async Task<IReadOnlyDictionary<string, ChatMembersCacheEntry>> GetCacheAsync(IEnumerable<string> keys)
    {
        keys = keys.ToArray();
        var chatIds = keys.Select(Guid.Parse).ToArray();

        var members = await _dbContext.Members
            .Where(x => chatIds.Contains(x.ChatId) && x.LastMessageId == null)
            .Select(x => new { x.ChatId, x.AccountId })
            .ToArrayAsync();
        var membersByChatId = members.ToLookup(x => x.ChatId.ToString());

        return keys.ToDictionary(
            x => x,
            x => new ChatMembersCacheEntry(Guid.Parse(x),
                membersByChatId[x].Select(member => member.AccountId).ToArray()));
    }
}
