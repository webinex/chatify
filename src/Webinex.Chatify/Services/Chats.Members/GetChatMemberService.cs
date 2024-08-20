using LinqToDB;
using Webinex.Chatify.Abstractions;
using Webinex.Chatify.DataAccess;
using Webinex.Chatify.Services.Chats.Caches;
using Webinex.Chatify.Services.Common.Caches;

namespace Webinex.Chatify.Services.Chats.Members;

internal interface IGetChatMemberService
{
    Task<IReadOnlyDictionary<Guid, IReadOnlyCollection<ChatMember>>> ByChatsAsync(
        IEnumerable<Guid> chatIds,
        bool? active = null);

    Task<IReadOnlyDictionary<Guid, string[]>> ActiveIdByChatIdAsync(IEnumerable<Guid> chatIds);
}

internal class GetChatMemberService : IGetChatMemberService
{
    private readonly IEntityCache<ChatMembersCacheEntry> _chatMemberCache;
    private readonly IChatifyDataConnectionFactory _dataConnectionFactory;

    public GetChatMemberService(
        IEntityCache<ChatMembersCacheEntry> chatMemberCache,
        IChatifyDataConnectionFactory dataConnectionFactory)
    {
        _chatMemberCache = chatMemberCache;
        _dataConnectionFactory = dataConnectionFactory;
    }

    public async Task<IReadOnlyDictionary<Guid, IReadOnlyCollection<ChatMember>>> ByChatsAsync(
        IEnumerable<Guid> chatIds,
        bool? active = null)
    {
        await using var connection = _dataConnectionFactory.Create();
        
        var queryable = connection.MemberRows.Where(x => chatIds.Contains(x.ChatId));
        if (active.HasValue)
            queryable = queryable.Where(x => (x.LastMessageId == null) == active);

        var rows = await queryable.ToArrayAsync();
        var rowByChatId = rows.ToLookup(x => x.ChatId);

        return chatIds.ToDictionary(id => id,
            id => (IReadOnlyCollection<ChatMember>)rowByChatId[id].Select(x => x.ToAbstraction()).ToArray());
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
        await using var connection = _dataConnectionFactory.Create();
        
        keys = keys.ToArray();
        var chatIds = keys.Select(Guid.Parse).ToArray();

        var members = await connection.MemberRows
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
