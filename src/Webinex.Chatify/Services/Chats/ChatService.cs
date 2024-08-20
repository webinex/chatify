using System.Data;
using LinqToDB;
using Webinex.Asky;
using Webinex.Chatify.Abstractions;
using Webinex.Chatify.Abstractions.Events;
using Webinex.Chatify.Common;
using Webinex.Chatify.DataAccess;
using Webinex.Chatify.Rows;
using Webinex.Chatify.Rows.Chats;
using Webinex.Chatify.Services.Chats.Messages;

namespace Webinex.Chatify.Services.Chats;

internal interface IChatService
{
    Task<Chat> AddAsync(AddChatArgs args);
    Task UpdateNameAsync(UpdateChatNameArgs args);
    Task<Chat[]> QueryAsync(ChatQuery query);
    Task<IReadOnlyCollection<Chat>> ByIdAsync(IEnumerable<Guid> ids, bool required = true);

    Task<IReadOnlyCollection<Chat>> GetAllAsync(
        FilterRule? filterRule = null,
        SortRule? sortRule = null,
        PagingRule? pagingRule = null);
}

internal class ChatService : IChatService
{
    private readonly IChatQueryService _queryService;
    private readonly IAddChatService _addChatService;
    private readonly IAskyFieldMap<ChatRow> _chatRowAskyFieldMap;
    private readonly IEventService _eventService;
    private readonly IChatifyDataConnectionFactory _dataConnectionFactory;

    public ChatService(
        IChatQueryService queryService,
        IAddChatService addChatService,
        IAskyFieldMap<ChatRow> chatRowAskyFieldMap,
        IEventService eventService,
        IChatifyDataConnectionFactory dataConnectionFactory)
    {
        _queryService = queryService;
        _addChatService = addChatService;
        _chatRowAskyFieldMap = chatRowAskyFieldMap;
        _eventService = eventService;
        _dataConnectionFactory = dataConnectionFactory;
    }

    public async Task<Chat> AddAsync(AddChatArgs args)
    {
        var row = await _addChatService.AddAsync(args);
        return ChatMapper.Map(row);
    }

    public async Task<Chat[]> QueryAsync(ChatQuery query)
    {
        return await _queryService.QueryAsync(query);
    }

    public async Task<IReadOnlyCollection<Chat>> ByIdAsync(IEnumerable<Guid> ids, bool required = true)
    {
        ids = ids.Distinct().ToArray();
        var connection = _dataConnectionFactory.Create();
        var result = await connection.ChatRows.Where(x => ids.Contains(x.Id)).ToArrayAsync();
        return result.Select(x => ChatMapper.Map(x)).ToArray();
    }

    public async Task UpdateNameAsync(UpdateChatNameArgs args)
    {
        await using var connection = _dataConnectionFactory.Create();
        await using var transaction = await connection.BeginTransactionAsync(IsolationLevel.ReadCommitted);

        var index = await connection.IncrementMetaIndexWithUpdLockAsync(args.Id);
        await connection.RenameChatAsync(args.Id, args.Name);
        var messageRow = ChatMessageRow.NewChatRenamed(args.Id, index, args.Name);
        var readForId = args.OnBehalfOf.IsSystem() ? null : args.OnBehalfOf.Id;
        await connection.SendMessageAsync(messageRow, readForId: readForId);

        await transaction.CommitAsync();

        _eventService.Push(new ChatNameChangedEvent(args.Id, args.Name,
            ChatMessageMapper.Map(messageRow, null), readForId));
        await _eventService.FlushAsync();
    }

    public async Task<IReadOnlyCollection<Chat>> GetAllAsync(
        FilterRule? filterRule = null,
        SortRule? sortRule = null,
        PagingRule? pagingRule = null)
    {
        await using var connection = _dataConnectionFactory.Create();
        var queryable = connection.ChatRows.AsQueryable();

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
