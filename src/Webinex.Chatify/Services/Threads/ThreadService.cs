using System.Data;
using LinqToDB;
using LinqToDB.Data;
using LinqToDB.DataProvider.SqlServer;
using Webinex.Asky;
using Webinex.Chatify.Abstractions;
using Webinex.Chatify.Abstractions.Events;
using Webinex.Chatify.Common;
using Webinex.Chatify.DataAccess;
using Webinex.Chatify.Rows.Threads;
using Thread = Webinex.Chatify.Abstractions.Thread;

namespace Webinex.Chatify.Services.Threads;

internal interface IThreadService
{
    Task<Thread> AddThreadAsync(AddThreadArgs args);
    Task UpdateThreadAsync(UpdateThreadArgs args);
    Task RemoveThreadAsync(string threadId);
    Task ArchiveThreadAsync(string threadId);
    Task SetThreadWatchAsync(AccountContext onBehalfOf, string threadId, string accountId, bool value);

    Task<ThreadMessage> SendThreadMessageAsync(SendThreadMessageArgs args);
    Task<int> ReadThreadMessageAsync(ReadThreadMessageArgs args);
    Task<Thread?> ThreadByIdAsync(AccountContext onBehalfOf, string id, Thread.Props props = Thread.Props.Default);
    Task<IReadOnlyDictionary<string, Thread?>> ThreadByIdAsync(IEnumerable<string> ids);
    Task<IReadOnlyCollection<ThreadMessage>> QueryAsync(ThreadMessageQuery query);

    Task AddThreadWatcherAsync(string threadId, string accountId);
    Task RemoveThreadWatcherAsync(string threadId, string accountId);

    Task<IReadOnlyDictionary<string, IReadOnlyCollection<string>>> WatchersByThreadIdAsync(
        IEnumerable<string> threadIds);

    Task<IReadOnlyCollection<Thread>> QueryAsync(ThreadWatchQuery query);
    Task<IReadOnlyDictionary<string, bool>> ThreadExistsAsync(IEnumerable<string> ids);
}

internal class ThreadService : IThreadService
{
    private readonly IChatifyDataConnectionFactory _dataConnectionFactory;
    private readonly IEventService _eventService;
    private readonly IAskyFieldMap<ThreadQueryView> _threadQueryViewFieldMap;

    public ThreadService(
        IChatifyDataConnectionFactory dataConnectionFactory,
        IEventService eventService,
        IAskyFieldMap<ThreadQueryView> threadQueryViewFieldMap)
    {
        _dataConnectionFactory = dataConnectionFactory;
        _eventService = eventService;
        _threadQueryViewFieldMap = threadQueryViewFieldMap;
    }

    public async Task<Thread> AddThreadAsync(AddThreadArgs args)
    {
        await using var connection = _dataConnectionFactory.Create();
        await using var transaction = await connection.BeginTransactionAsync(IsolationLevel.ReadCommitted);

        var threadRow = ThreadRow.New(args.Id, args.Name, args.OnBehalfOf.Id);
        var threadMetaRow = new ThreadMetaRow(threadRow.Id, null);
        var threadWatcherRows
            = args.Watchers?.Select(watcherId => ThreadWatchRow.New(threadRow.Id, watcherId)).ToArray() ??
              Array.Empty<ThreadWatchRow>();

        await connection.InsertAsync(threadRow);
        await connection.InsertAsync(threadMetaRow);
        await connection.BulkCopyAsync(threadWatcherRows);

        await transaction.CommitAsync();
        await _eventService.PushAndFlushAsync(new ThreadCreatedEvent(threadRow.Id, threadRow.Name, args.OnBehalfOf,
            threadWatcherRows.Select(x => x.AccountId)));

        return ThreadMapper.Map(threadRow);
    }

    public async Task UpdateThreadAsync(UpdateThreadArgs args)
    {
        await using var connection = _dataConnectionFactory.Create();
        await connection.ThreadRows.Where(x => x.Id == args.Id)
            .Set(x => x.Name, args.Name)
            .UpdateAsync()
            .AssertResult(1);
        await _eventService.PushAndFlushAsync(new ThreadUpdatedEvent(args.Id, args.Name));
    }

    public async Task RemoveThreadAsync(string threadId)
    {
        await using var connection = _dataConnectionFactory.Create();
        await connection.ThreadRows.Where(x => x.Id == threadId).DeleteAsync().AssertResult(1);
        await _eventService.PushAndFlushAsync(new ThreadRemovedEvent(threadId));
    }

    public async Task ArchiveThreadAsync(string threadId)
    {
        await using var connection = _dataConnectionFactory.Create();
        await connection.ThreadRows.Where(x => x.Id == threadId && !x.Archived)
            .Set(x => x.Archived, true)
            .UpdateAsync()
            .AssertResult(1);
        await _eventService.PushAndFlushAsync(new ThreadArchivedEvent(threadId));
    }

    public async Task AddThreadWatcherAsync(string threadId, string accountId)
    {
        await using var connection = _dataConnectionFactory.Create();
        var lastMessageRow = await connection.ThreadMessageRows.OrderByDescending(x => x.Id)
            .FirstOrDefaultAsync(x => x.ThreadId == threadId);
        await connection.InsertAsync(ThreadWatchRow.New(threadId, accountId, lastMessageRow?.Id));

        var lastMessage = lastMessageRow != null
            ? ThreadMessageMapper.Map(lastMessageRow)
            : null;

        await _eventService.PushAndFlushAsync(new ThreadWatchAddedEvent(threadId, accountId, lastMessage,
            lastMessageRow?.Id));
    }

    public async Task RemoveThreadWatcherAsync(string threadId, string accountId)
    {
        await using var connection = _dataConnectionFactory.Create();
        await connection.ThreadWatchRows.Where(x => x.ThreadId == threadId && x.AccountId == accountId)
            .DeleteAsync().AssertResult(1);
        await _eventService.PushAndFlushAsync(new ThreadWatchRemovedEvent(threadId, accountId));
    }

    public async Task<IReadOnlyDictionary<string, IReadOnlyCollection<string>>> WatchersByThreadIdAsync(
        IEnumerable<string> threadIds)
    {
        threadIds = threadIds.Distinct().ToArray();
        await using var connection = _dataConnectionFactory.Create();
        var rows = await connection.ThreadWatchRows.Where(x => threadIds.Contains(x.ThreadId)).ToArrayAsync();

        return threadIds.ToDictionary(id => id,
            id => (IReadOnlyCollection<string>)rows.Where(x => x.ThreadId == id).Select(x => x.AccountId).ToArray());
    }

    public async Task SetThreadWatchAsync(AccountContext onBehalfOf, string threadId, string accountId, bool value)
    {
        if (!onBehalfOf.IsSystem() && onBehalfOf.Id != accountId)
            throw new InvalidOperationException("Unable to set watch for another account not on behalf of system");

        ThreadWatchRow? watch;

        await using (var connection = _dataConnectionFactory.Create())
        {
            watch = await connection.ThreadWatchRows.FirstOrDefaultAsync(x =>
                x.ThreadId == threadId && x.AccountId == accountId);
        }

        if (watch != null && !value)
        {
            await RemoveThreadWatcherAsync(threadId, accountId);
            return;
        }

        if (watch == null && value)
        {
            await AddThreadWatcherAsync(threadId, accountId);
        }
    }

    public async Task<ThreadMessage> SendThreadMessageAsync(SendThreadMessageArgs args)
    {
        await using var connection = _dataConnectionFactory.Create();
        await using var transaction = await connection.BeginTransactionAsync(IsolationLevel.ReadCommitted);

        var threadMetaRow = await connection.ThreadMetaRows.AsSqlServer().WithUpdLock()
            .FirstAsync(x => x.ThreadId == args.ThreadId);
        threadMetaRow.Increment();
        await connection.UpdateAsync(threadMetaRow).AssertResult(1);

        var messageRow = ThreadMessageRow.New(new ThreadMessageId(args.ThreadId, threadMetaRow.LastIndex!.Value),
            args.OnBehalfOf.Id, args.Body);

        await connection.InsertAsync(messageRow);
        await connection.ThreadRows.Where(x => x.Id == args.ThreadId).Set(x => x.LastMessageId, messageRow.Id)
            .UpdateAsync().AssertResult(1);

        var readForId = !args.OnBehalfOf.IsSystem() ? args.OnBehalfOf.Id : null;

        if (readForId != null)
            await connection.ThreadWatchRows
                .Where(x => x.ThreadId == args.ThreadId && x.AccountId == readForId)
                .Set(x => x.LastReadMessageId, messageRow.Id)
                .UpdateAsync();

        await transaction.CommitAsync();
        await _eventService.PushAndFlushAsync(new ThreadMessageSendEvent(messageRow.Id, messageRow.ThreadId,
            messageRow.Body(), args.OnBehalfOf, messageRow.SentAt, readForId));

        return ThreadMessageMapper.Map(messageRow);
    }

    public async Task<int> ReadThreadMessageAsync(ReadThreadMessageArgs args)
    {
        await using var connection = _dataConnectionFactory.Create();

        var dbResult = await connection.ThreadWatchRows
            .Where(x => x.ThreadId == args.ThreadId && x.AccountId == args.OnBehalfOf.Id)
            .Set(x => x.LastReadMessageId,
                x => x.LastReadMessageId == null || x.LastReadMessageId.CompareTo(args.MessageId) < 0
                    ? args.MessageId
                    : x.LastReadMessageId)
            .UpdateWithOutputAsync((deleted, @new) => new { PreviousLastReadMessageId = deleted.LastReadMessageId });

        var previousLastReadMessageId = dbResult.Single().PreviousLastReadMessageId;
        var previousReadIndex = previousLastReadMessageId != null
            ? ThreadMessageId.Parse(previousLastReadMessageId).Index
            : -1;
        var newReadIndex = ThreadMessageId.Parse(args.MessageId).Index;
        var readCount = Math.Max(newReadIndex - previousReadIndex, 0);

        if (readCount > 0)
            await _eventService.PushAndFlushAsync(new ThreadMessageReadEvent(args.ThreadId, args.MessageId,
                args.OnBehalfOf.Id, readCount));

        return readCount;
    }

    public async Task<IReadOnlyCollection<Thread>> QueryAsync(ThreadWatchQuery query)
    {
        await using var connection = _dataConnectionFactory.Create();
        var queryable = ThreadQueryView.Queryable(connection, query.OnBehalfOf, query.Props, watchRequired: true);
        if (query.FilterRule != null) queryable = queryable.Where(_threadQueryViewFieldMap, query.FilterRule);
        if (query.SortRule != null) queryable = queryable.SortBy(_threadQueryViewFieldMap, query.SortRule);
        if (query.PagingRule != null) queryable = queryable.PageBy(query.PagingRule);

        var dbResult = await queryable.ToArrayAsync();
        return dbResult.Select(view => ThreadMapper.Map(view, query.Props)).ToArray();
    }

    public async Task<IReadOnlyDictionary<string, bool>> ThreadExistsAsync(IEnumerable<string> ids)
    {
        ids = ids.Distinct().ToArray();
        if (!ids.Any()) return new Dictionary<string, bool>();
        
        await using var connection = _dataConnectionFactory.Create();
        var dbResult = await connection.ThreadRows.Where(x => ids.Contains(x.Id)).Select(x => x.Id).ToArrayAsync();
        return ids.ToDictionary(threadId => threadId, threadId => dbResult.Contains(threadId));
    }

    public async Task<Thread?> ThreadByIdAsync(
        AccountContext onBehalfOf,
        string id,
        Thread.Props props = Thread.Props.Default)
    {
        await using var connection = _dataConnectionFactory.Create();
        var queryable = ThreadQueryView.Queryable(connection, onBehalfOf, props)
            .Where(_threadQueryViewFieldMap, FilterRule.Eq("id", id));
        var dbResult = await queryable.FirstOrDefaultAsync();
        return dbResult != null ? ThreadMapper.Map(dbResult, props) : null;
    }

    public async Task<IReadOnlyDictionary<string, Thread?>> ThreadByIdAsync(IEnumerable<string> ids)
    {
        ids = ids.Distinct().ToArray();
        await using var connection = _dataConnectionFactory.Create();
        var threadRows = await connection.ThreadRows.Where(x => ids.Contains(x.Id)).ToArrayAsync();

        return ids.ToDictionary(id => id, id =>
        {
            var row = threadRows.FirstOrDefault(x => x.Id == id);
            return row != null ? ThreadMapper.Map(row) : null;
        });
    }

    public async Task<IReadOnlyCollection<ThreadMessage>> QueryAsync(ThreadMessageQuery query)
    {
        await using var connection = _dataConnectionFactory.Create();
        var queryable = ThreadMessageQueryView.Queryable(connection, query.Props, query.OnBehalfOf)
            .Where(x => x.Message.ThreadId == query.ThreadId);

        queryable = query.SentAtAsc
            ? queryable.OrderBy(x => x.Message.SentAt)
            : queryable.OrderByDescending(x => x.Message.SentAt);

        queryable = queryable.PageBy(query.PagingRule);
        var dbResult = await queryable.ToArrayAsync();
        return dbResult.Select(x => ThreadMessageMapper.Map(x, query.Props)).ToArray();
    }
}