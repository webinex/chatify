using System.Collections.ObjectModel;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Webinex.Asky;
using Webinex.Chatify.Abstractions;
using Webinex.Chatify.Common.Events;
using Webinex.Chatify.DataAccess;
using Webinex.Chatify.Queries;
using Webinex.Chatify.Tasks;
using Webinex.Chatify.Types;
using Webinex.Chatify.Types.Events;
using Webinex.Coded;

namespace Webinex.Chatify;

internal class Chatify : IChatify
{
    private readonly ChatifyDbContext _dbContext;
    private readonly IEventService _eventService;
    private readonly IAskyFieldMap<ChatQueryHandler.View> _chatQueryViewFieldMap;
    private readonly IAskyFieldMap<MessageQueryHandler.View> _messageQueryViewFieldMap;
    private readonly IChatifyQueue _queue;

    public Chatify(
        ChatifyDbContext dbContext,
        IEventService eventService,
        IAskyFieldMap<ChatQueryHandler.View> chatQueryViewFieldMap,
        IAskyFieldMap<MessageQueryHandler.View> messageQueryViewFieldMap,
        IChatifyQueue queue)
    {
        _dbContext = dbContext;
        _eventService = eventService;
        _chatQueryViewFieldMap = chatQueryViewFieldMap;
        _messageQueryViewFieldMap = messageQueryViewFieldMap;
        _queue = queue;
    }

    public async Task<IReadOnlyCollection<Chat>> AddChatsAsync(IEnumerable<AddChatCommand> commands)
    {
        return await new AddChatCommandHandler(_dbContext, _eventService, commands.ToArray()).ExecuteAsync();
    }

    public async Task<Message[]> AddMessagesAsync(IEnumerable<AddMessageCommand> commands)
    {
        return await new AddMessageCommandHandler(_dbContext, _eventService, commands.ToArray()).ExecuteAsync();
    }

    public async Task AddMembersAsync(IEnumerable<AddMemberCommand> commands)
    {
        commands = commands.ToArray();

        var queue = commands.Where(x => x.WithHistory).ToArray();
        await _queue.EnqueueAsync(queue.Select(x =>
            new AddMemberTask(x.ChatId, x.AccountId, x.OnBehalfOf.Id, DateTimeOffset.UtcNow)));

        var sync = commands.Except(queue).ToArray();
        if (!sync.Any())
            return;

        var members = sync.Select(x => Member.NewAdded(_eventService, x.ChatId, x.AccountId, x.OnBehalfOf.Id))
            .ToArray();
        await _dbContext.Members.AddRangeAsync(members);
        await _eventService.FlushAsync();
        await _dbContext.SaveChangesAsync();
    }

    public async Task RemoveMembersAsync(IEnumerable<RemoveMemberCommand> commands)
    {
        commands = commands.ToArray();

        var queue = commands.Where(x => x.DeleteHistory).ToArray();
        await _queue.EnqueueAsync(queue.Select(x => new RemoveMemberTask(x.ChatId, x.AccountId)).ToArray());


        var sync = commands.Except(queue).ToArray();
        if (!sync.Any())
            return;

        Expression<Func<Member, bool>> expression;
        if (sync.Length == 1)
        {
            expression = MemberById(sync.First().ChatId, sync.First().AccountId);
        }
        else
        {
            var expressions = sync.Select(x => MemberById(x.ChatId, x.AccountId)).ToArray();
            expression = expressions.Skip(1).Aggregate(expressions.ElementAt(0), OrElse);
        }

        var members = await _dbContext.Members.Where(expression).ToArrayAsync();
        _dbContext.Members.RemoveRange(members);
        foreach (var member in members)
            _eventService.Push(new MemberRemovedEvent(member.ChatId, member.AccountId, false));

        await _eventService.FlushAsync();
        await _dbContext.SaveChangesAsync();
    }

    private static Expression<Func<Member, bool>> MemberById(Guid chatId, string accountId)
    {
        return x => x.ChatId == chatId && x.AccountId == accountId;
    }

    private static Expression<Func<T, bool>> OrElse<T>(
        Expression<Func<T, bool>> left,
        Expression<Func<T, bool>> right)
    {
        var param = Expression.Parameter(typeof(T), "x");
        var body = Expression.OrElse(
            Expression.Invoke(left, param),
            Expression.Invoke(right, param)
        );
        var lambda = Expression.Lambda<Func<T, bool>>(body, param);
        return lambda;
    }

    public async Task<ChatQueryResult> QueryAsync(ChatQuery query)
    {
        return await new ChatQueryHandler(_dbContext, _chatQueryViewFieldMap, query).QueryAsync();
    }

    public async Task<MessageQueryResult> QueryAsync(MessageQuery query)
    {
        return await new MessageQueryHandler(_dbContext, _messageQueryViewFieldMap, query).QueryAsync();
    }

    public async Task<IReadOnlyCollection<Chat>> ChatByIdAsync(IEnumerable<Guid> chatIds)
    {
        var queryable = _dbContext.Chats.AsQueryable();
        queryable = queryable.Where(x => chatIds.Contains(x.Id));
        return await queryable.ToArrayAsync();
    }

    public async Task<IReadOnlyDictionary<Guid, IReadOnlyCollection<Member>>> MembersAsync(IEnumerable<Guid> chatIds)
    {
        var queryable = _dbContext.Members.Where(x => chatIds.Contains(x.ChatId));
        var result = await queryable.ToArrayAsync();
        return chatIds.ToDictionary(id => id,
            id => (IReadOnlyCollection<Member>)result.Where(x => x.ChatId == id).ToArray());
    }

    public async Task<IReadOnlyCollection<Account>> AccountByIdAsync(IEnumerable<string>? ids = null)
    {
        ids = ids?.Distinct().ToArray();

        if (ids != null && !ids.Any())
            return new Collection<Account>();

        var queryable = _dbContext.Accounts.AsQueryable();

        if (ids != null)
            queryable = queryable.Where(x => ids.Contains(x.Id));

        if (ids == null)
            queryable = queryable.Where(x => x.Id != AccountId.SYSTEM);

        return await queryable.ToArrayAsync();
    }

    public async Task ReadAsync(ReadCommand command)
    {
        var deliveries =
            await _dbContext.Deliveries.Where(x => x.ToId == command.OnBehalfOf.Id && command.Ids.Contains(x.MessageId))
                .ToArrayAsync();

        foreach (var delivery in deliveries)
        {
            delivery.MarkRead(_eventService);
        }

        await _eventService.FlushAsync();
        await _dbContext.SaveChangesAsync();
    }

    public async Task<Account[]> AddAccountsAsync(IEnumerable<AddAccountCommand> commands)
    {
        var accounts = commands.Select(x => new Account(x.Id, x.Name, x.Avatar, true)).ToArray();
        await _dbContext.Accounts.AddRangeAsync(accounts);
        await _dbContext.SaveChangesAsync();
        return accounts;
    }

    public async Task<Account[]> UpdateAccountsAsync(IEnumerable<UpdateAccountCommand> commands)
    {
        commands = commands.ToArray();
        var ids = commands.Select(x => x.Id).Distinct().ToArray();
        var accounts = await _dbContext.Accounts.Where(x => ids.Contains(x.Id)).ToArrayAsync();
        var accountById = accounts.ToDictionary(x => x.Id);

        foreach (var command in commands)
        {
            var account = accountById[command.Id];
            account.UpdateName(command.Name);
            account.UpdateAvatar(command.Avatar);
            account.UpdateActive(command.Active);
        }

        return accounts;
    }
}