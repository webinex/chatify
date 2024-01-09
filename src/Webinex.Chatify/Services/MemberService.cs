using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Webinex.Asky;
using Webinex.Chatify.Abstractions;
using Webinex.Chatify.Abstractions.Events;
using Webinex.Chatify.Common;
using Webinex.Chatify.DataAccess;
using Webinex.Chatify.Rows;
using Webinex.Chatify.Services.Caches;
using Webinex.Chatify.Services.Caches.Common;
using Webinex.Chatify.Services.Tasks;

namespace Webinex.Chatify.Services;

internal interface IMemberService
{
    Task<IReadOnlyDictionary<Guid, IReadOnlyCollection<Member>>> ByChatsAsync(IEnumerable<Guid> chatIds);
    Task RemoveRangeAsync(IEnumerable<RemoveMemberArgs> commands);
    Task AddRangeAsync(IEnumerable<AddMemberArgs> commands);
    Task<IReadOnlyDictionary<Guid, string[]>> IdByChatIdAsync(IEnumerable<Guid> chatIds);
}

internal class MemberService : IMemberService
{
    private readonly ChatifyDbContext _dbContext;
    private readonly IChatifyQueue _queue;
    private readonly IEventService _eventService;
    private readonly IEntityCache<ChatMembersCacheEntry> _chatMemberCache;

    public MemberService(
        ChatifyDbContext dbContext,
        IChatifyQueue queue,
        IEventService eventService,
        IEntityCache<ChatMembersCacheEntry> chatMemberCache)
    {
        _dbContext = dbContext;
        _queue = queue;
        _eventService = eventService;
        _chatMemberCache = chatMemberCache;
    }

    public async Task<IReadOnlyDictionary<Guid, IReadOnlyCollection<Member>>> ByChatsAsync(IEnumerable<Guid> chatIds)
    {
        var queryable = _dbContext.Members.Where(x => chatIds.Contains(x.ChatId));
        var rows = await queryable.Where(x => chatIds.Contains(x.ChatId)).ToArrayAsync();
        var rowByChatId = rows.ToLookup(x => x.ChatId);

        return chatIds.ToDictionary(id => id,
            id => (IReadOnlyCollection<Member>)rowByChatId[id].Select(x => x.ToAbstraction()).ToArray());
    }

    public async Task RemoveRangeAsync(IEnumerable<RemoveMemberArgs> commands)
    {
        commands = commands.ToArray();
        await _queue.EnqueueAsync(commands.Select(x => new RemoveMemberTask(x.ChatId, x.AccountId)).ToArray());

        // await _queue.EnqueueAsync(queue.Select(x => new RemoveMemberTask(x.ChatId, x.AccountId)).ToArray());
        //
        // var sync = commands.Except(queue).ToArray();
        // if (!sync.Any())
        //     return;
        //
        // Expression<Func<MemberRow, bool>> expression;
        // if (sync.Length == 1)
        // {
        //     expression = MemberById(sync.First().ChatId, sync.First().AccountId);
        // }
        // else
        // {
        //     var expressions = sync.Select(x => MemberById(x.ChatId, x.AccountId)).ToArray();
        //     expression = expressions.Skip(1).Aggregate(expressions.ElementAt(0), ExpressionUtil.OrElse);
        // }
        //
        // var members = await _dbContext.Members.Where(expression).ToArrayAsync();
        // _dbContext.Members.RemoveRange(members);
        // foreach (var member in members)
        //     _eventService.Push(new MemberRemovedEvent(member.ChatId, member.AccountId, false));
        //
        // await _eventService.FlushAsync();
        // await _dbContext.SaveChangesAsync();
        // await _chatMemberCache.InvalidateAsync(members.Select(x => x.ChatId.ToString()).Distinct().ToArray());
    }

    public async Task AddRangeAsync(IEnumerable<AddMemberArgs> commands)
    {
        commands = commands.ToArray();
        // var queue = commands.Where(x => x.History).ToArray();
        // var sync = commands.Except(queue).ToArray();

        await EnqueueAsync(commands);
        // await AddSyncAsync(sync);
    }

    public async Task<IReadOnlyDictionary<Guid, string[]>> IdByChatIdAsync(IEnumerable<Guid> chatIds)
    {
        var keys = chatIds.Select(x => x.ToString()).ToArray();
        var entries = await _chatMemberCache.GetOrCreateAsync(
            keys,
            GetCacheAsync);
        return entries.ToDictionary(x => x.Value.ChatId, x => x.Value.MemberIds.ToArray());
    }

    private async Task<IReadOnlyDictionary<string, ChatMembersCacheEntry>> GetCacheAsync(IEnumerable<string> keys)
    {
        keys = keys.ToArray();
        var chatIds = keys.Select(Guid.Parse).ToArray();

        var members = await _dbContext.Members.Where(x => chatIds.Contains(x.ChatId))
            .Select(x => new { x.ChatId, x.AccountId })
            .ToArrayAsync();
        var membersByChatId = members.ToLookup(x => x.ChatId.ToString());

        return keys.ToDictionary(
            x => x,
            x => new ChatMembersCacheEntry(Guid.Parse(x),
                membersByChatId[x].Select(member => member.AccountId).ToArray()));
    }

    private async Task EnqueueAsync(IEnumerable<AddMemberArgs> commands)
    {
        commands = commands.ToArray();
        if (!commands.Any()) return;

        await _queue.EnqueueAsync(commands.Select(x =>
            new AddMemberTask(x.ChatId, x.AccountId, x.OnBehalfOf.Id, DateTimeOffset.UtcNow)));
    }

    // private async Task AddSyncAsync(IEnumerable<AddMemberArgs> commands)
    // {
    //     commands = commands.ToArray();
    //     if (!commands.Any()) return;
    //
    //     var members = commands.Select(x =>
    //         MemberRow.NewInitial(_eventService, x.ChatId, x.AccountId, x.OnBehalfOf.Id, DateTimeOffset.UtcNow)).ToArray();
    //
    //     await _dbContext.Members.AddRangeAsync(members);
    //     await _eventService.FlushAsync();
    //     await _dbContext.SaveChangesAsync();
    //     await _chatMemberCache.InvalidateAsync(members.Select(x => x.ChatId.ToString()).Distinct().ToArray());
    // }

    // private static Expression<Func<MemberRow, bool>> MemberById(Guid chatId, string accountId)
    // {
    //     return x => x.ChatId == chatId && x.AccountId == accountId;
    // }
}

internal static class MemberServiceExtensions
{
    public static async Task<IReadOnlyCollection<string>> IdByChatIdAsync(
        this IMemberService memberService,
        Guid chatId)
    {
        var result = await memberService.IdByChatIdAsync(new[] { chatId });
        return result.Values.First();
    }
}
