using Microsoft.EntityFrameworkCore;
using Webinex.Chatify.Abstractions;
using Webinex.Chatify.Abstractions.Events;
using Webinex.Chatify.Common;
using Webinex.Chatify.DataAccess;
using Webinex.Chatify.Rows;
using Webinex.Chatify.Services.Caches;
using Webinex.Chatify.Services.Caches.Common;
using Webinex.Chatify.Services.Messages;

namespace Webinex.Chatify.Services.Tasks;

internal class AddMemberJob : IJob<AddMemberTask>
{
    private readonly IEventService _eventService;
    private readonly ChatifyDbContext _dbContext;
    private readonly IEntityCache<ChatMembersCacheEntry> _chatMemberCache;
    private readonly IMemberService _memberService;

    public AddMemberJob(
        IEventService eventService,
        ChatifyDbContext dbContext,
        IEntityCache<ChatMembersCacheEntry> chatMemberCache,
        IMemberService memberService)
    {
        _eventService = eventService;
        _dbContext = dbContext;
        _chatMemberCache = chatMemberCache;
        _memberService = memberService;
    }

    public async Task InvokeAsync(AddMemberTask task)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync();
        var members = await _memberService.IdByChatIdAsync(task.ChatId);

        await AddMemberAsync(task);
        var message = await AddMessageAsync(task);
        await AddDeliveriesAsync(task, message, members);
        await AddChatActivityAsync(task, message);
        await UpdateChatActivityAsync(members, message);

        _eventService.Push(new MemberAddedEvent(task.ChatId, task.AccountId, MessageMapper.Map(message)));
        await _eventService.FlushAsync();
        await _chatMemberCache.InvalidateAsync(task.ChatId.ToString());
        await transaction.CommitAsync();
        await _chatMemberCache.InvalidateAsync(task.ChatId.ToString());
    }

    private async Task AddMemberAsync(AddMemberTask task)
    {
        _dbContext.Members.Add(MemberRow.NewByJob(task.ChatId, task.AccountId, task.AddedById, task.AddedAt));
        await _dbContext.SaveChangesAsync();
    }

    private async Task<MessageRow> AddMessageAsync(AddMemberTask task)
    {
        var message = MessageRow.NewMemberAddedByJob(task.ChatId, task.AddedById, task.AccountId);
        _dbContext.Messages.Add(message);
        await _dbContext.SaveChangesAsync();
        return message;
    }

    private async Task AddDeliveriesAsync(AddMemberTask task, MessageRow message, IReadOnlyCollection<string> members)
    {
        await CopyHistoryAsync(task);

        var deliveries = members.Select(x => DeliveryRow.NewMemberAddedByJob(task.ChatId, message.Id, x)).ToArray();
        await _dbContext.Deliveries.AddRangeAsync(deliveries);
    }

    private async Task CopyHistoryAsync(AddMemberTask task)
    {
        await _dbContext.Database.ExecuteSqlRawAsync(
            @"
INSERT INTO [chatify].[Deliveries](ChatId, MessageId, ToId, FromId, [Read])
SELECT message.ChatId, message.Id, @p0, message.AuthorId, 1
    FROM [chatify].[Messages] message
    LEFT OUTER JOIN [chatify].[Deliveries] delivery on message.Id = delivery.MessageId and delivery.ToId = @p0
WHERE delivery.ToId is null and message.ChatId = @p1", task.AccountId, task.ChatId);
    }

    private async Task AddChatActivityAsync(AddMemberTask task, MessageRow message)
    {
        await _dbContext.ChatActivities.AddAsync(new ChatActivityRow(task.ChatId, task.AccountId,
            message.AuthorId, message.Id));
        await _dbContext.SaveChangesAsync();
    }

    private async Task UpdateChatActivityAsync(IReadOnlyCollection<string> members, MessageRow message)
    {
        await _dbContext.ChatActivities.Where(x => x.ChatId == message.ChatId && members.Contains(x.AccountId))
            .ExecuteUpdateAsync(x =>
                x.SetProperty(a => a.LastMessageId, message.Id)
                    .SetProperty(a => a.LastMessageFromId, message.AuthorId));
    }
}
