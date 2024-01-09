using Microsoft.EntityFrameworkCore;
using Webinex.Chatify.Abstractions.Events;
using Webinex.Chatify.Common;
using Webinex.Chatify.DataAccess;
using Webinex.Chatify.Services.Caches;
using Webinex.Chatify.Services.Caches.Common;

namespace Webinex.Chatify.Services.Tasks;

internal class RemoveMemberJob : IJob<RemoveMemberTask>
{
    private readonly ChatifyDbContext _dbContext;
    private readonly IEventService _eventService;
    private readonly IEntityCache<ChatMembersCacheEntry> _chatMembersCache;

    public RemoveMemberJob(
        ChatifyDbContext dbContext,
        IEventService eventService,
        IEntityCache<ChatMembersCacheEntry> chatMembersCache)
    {
        _dbContext = dbContext;
        _eventService = eventService;
        _chatMembersCache = chatMembersCache;
    }

    public async Task InvokeAsync(RemoveMemberTask task)
    {
        var member = await _dbContext.Members.FindAsync(task.ChatId, task.AccountId);
        var chatActivity = await _dbContext.ChatActivities.FindAsync(member!.ChatId, member.AccountId);

        await using var transaction = await _dbContext.Database.BeginTransactionAsync();
        _dbContext.Members.Remove(member);
        _dbContext.ChatActivities.Remove(chatActivity!);
        await _dbContext.Deliveries.Where(x => x.ChatId == member!.ChatId && x.ToId == member.AccountId)
            .ExecuteDeleteAsync();
        await _dbContext.SaveChangesAsync();

        _eventService.Push(new MemberRemovedEvent(task.ChatId, task.AccountId, true));
        await _eventService.FlushAsync();
        await _chatMembersCache.InvalidateAsync(task.ChatId.ToString());
        await transaction.CommitAsync();
        await _chatMembersCache.InvalidateAsync(task.ChatId.ToString());
    }
}
