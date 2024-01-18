using Microsoft.EntityFrameworkCore;
using Webinex.Asky;
using Webinex.Chatify.Abstractions;
using Webinex.Chatify.DataAccess;
using Webinex.Chatify.Rows;

namespace Webinex.Chatify.Services.Chats;

internal interface IChatService
{
    Task<IReadOnlyCollection<Chat>> AddAsync(IEnumerable<AddChatArgs> commands);
    Task<Chat[]> QueryAsync(ChatQuery query);
    Task<IReadOnlyCollection<Chat>> ByIdAsync(IEnumerable<Guid> ids, bool required = true);

    Task<IReadOnlyCollection<Chat>> GetAllAsync(
        FilterRule? filterRule = null,
        SortRule? sortRule = null,
        PagingRule? pagingRule = null);
}

internal class ChatService : IChatService
{
    private readonly ChatifyDbContext _dbContext;
    private readonly IChatQueryService _queryService;
    private readonly IAddChatService _addChatService;
    private readonly IAskyFieldMap<ChatRow> _chatRowAskyFieldMap;

    public ChatService(
        ChatifyDbContext dbContext,
        IChatQueryService queryService,
        IAddChatService addChatService,
        IAskyFieldMap<ChatRow> chatRowAskyFieldMap)
    {
        _dbContext = dbContext;
        _queryService = queryService;
        _addChatService = addChatService;
        _chatRowAskyFieldMap = chatRowAskyFieldMap;
    }


    public async Task<IReadOnlyCollection<Chat>> AddAsync(IEnumerable<AddChatArgs> commands)
    {
        var result = await _addChatService.AddRangeAsync(commands);
        return result.Select(x => ChatMapper.Map(x)).ToArray();
    }

    public async Task<Chat[]> QueryAsync(ChatQuery query)
    {
        return await _queryService.QueryAsync(query);
    }

    public async Task<IReadOnlyCollection<Chat>> ByIdAsync(IEnumerable<Guid> ids, bool required = true)
    {
        ids = ids.Distinct().ToArray();
        var result = await _dbContext.Chats.FindManyNoTrackingAsync(ids, required);
        return result.Select(x => ChatMapper.Map(x)).ToArray();
    }

    public async Task<IReadOnlyCollection<Chat>> GetAllAsync(
        FilterRule? filterRule = null,
        SortRule? sortRule = null,
        PagingRule? pagingRule = null)
    {
        var queryable = _dbContext.Chats.AsQueryable();
        
        if (filterRule != null)
            queryable = queryable.Where(_chatRowAskyFieldMap, filterRule);

        if (sortRule != null)
            queryable = queryable.SortBy(_chatRowAskyFieldMap, sortRule);

        if (pagingRule != null)
            queryable = queryable.PageBy(pagingRule);

        var rows = await queryable.ToArrayAsync();
        return rows.Select(x => ChatMapper.Map(x)).ToArray();
    }
}
