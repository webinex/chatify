using Microsoft.EntityFrameworkCore;
using Webinex.Chatify.Abstractions;
using Webinex.Chatify.DataAccess;

namespace Webinex.Chatify.Services.Chats;

internal interface IChatService
{
    Task<IReadOnlyCollection<Chat>> AddAsync(IEnumerable<AddChatArgs> commands);
    Task<Chat[]> QueryAsync(ChatQuery query);
    Task<IReadOnlyCollection<Chat>> ByIdAsync(IEnumerable<Guid> ids);
}

internal class ChatService : IChatService
{
    private readonly ChatifyDbContext _dbContext;
    private readonly IChatQueryService _queryService;
    private readonly IAddChatService _addChatService;

    public ChatService(
        ChatifyDbContext dbContext,
        IChatQueryService queryService,
        IAddChatService addChatService)
    {
        _dbContext = dbContext;
        _queryService = queryService;
        _addChatService = addChatService;
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

    public async Task<IReadOnlyCollection<Chat>> ByIdAsync(IEnumerable<Guid> ids)
    {
        var queryable = _dbContext.Chats.AsQueryable();
        queryable = queryable.Where(x => ids.Contains(x.Id));
        var result = await queryable.ToArrayAsync();
        return result.Select(x => ChatMapper.Map(x)).ToArray();
    }
}
