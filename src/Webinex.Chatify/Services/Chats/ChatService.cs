using System.Data;
using System.Text.Json;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Webinex.Asky;
using Webinex.Chatify.Abstractions;
using Webinex.Chatify.Abstractions.Events;
using Webinex.Chatify.Common;
using Webinex.Chatify.DataAccess;
using Webinex.Chatify.Rows;
using Webinex.Chatify.Services.Messages;
using File = Webinex.Chatify.Abstractions.File;

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
    private readonly ChatifyDbContext _dbContext;
    private readonly IChatQueryService _queryService;
    private readonly IAddChatService _addChatService;
    private readonly IAskyFieldMap<ChatRow> _chatRowAskyFieldMap;
    private readonly IEventService _eventService;

    public ChatService(
        ChatifyDbContext dbContext,
        IChatQueryService queryService,
        IAddChatService addChatService,
        IAskyFieldMap<ChatRow> chatRowAskyFieldMap,
        IEventService eventService)
    {
        _dbContext = dbContext;
        _queryService = queryService;
        _addChatService = addChatService;
        _chatRowAskyFieldMap = chatRowAskyFieldMap;
        _eventService = eventService;
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
        var result = await _dbContext.Chats.FindManyNoTrackingAsync(ids, required);
        return result.Select(x => ChatMapper.Map(x)).ToArray();
    }

    public async Task UpdateNameAsync(UpdateChatNameArgs args)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted);
        var now = DateTimeOffset.UtcNow;
        var text
            = $"chatify://chat-name-changed::{JsonSerializer.Serialize(new { NewName = args.Name }, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase })}";

        var parameters = new List<SqlParameter>
        {
            new("chatId", args.Id),
            new("name", args.Name),
            new("authorId", args.OnBehalfOf.Id),
            new("now", now),
            new("ticks", now.Ticks),
            new("messageText", text),
            new("systemId", AccountContext.System.Id),
        };

#pragma warning disable EF1002
        var result = (await _dbContext.Database.SqlQueryRaw<Result>(
#pragma warning restore EF1002
            $"""
             declare @index int;

             {ChatMetaRow.FormatSqlIncrement(("@chatId", "@index"))}

             update chatify.Chats set Name = @name where Id = @chatId

             declare @messageId char(65) = CONCAT(@chatId, '-', REPLACE(STR(@index, 9), SPACE(1), '0'), '-', @ticks);
             {MessageRow.FormatSqlInsert(("@messageId", "@chatId", "@messageText", "@systemId", "@now", "@index", null))}
             
             update chatify.ChatActivities
                set LastMessageId = @messageId, LastMessageIndex = @index, LastMessageFromId = @systemId
                where ChatId = @chatId and Active = 1

             select @index as [Index], @messageId as MessageId;
             """,
            parameters.ToArray()).ToArrayAsync()).First();

        await transaction.CommitAsync();

        var row = new MessageRow(result.MessageId, args.Id, AccountContext.System.Id, now, text, Array.Empty<File>());

        _eventService.Push(new ChatNameChangedEvent(args.Id, args.Name,
            MessageMapper.Map(row, null)));
        await _eventService.FlushAsync();
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

    private class Result
    {
        public int Index { get; set; }
        public string MessageId { get; set; } = null!;
    }
}
