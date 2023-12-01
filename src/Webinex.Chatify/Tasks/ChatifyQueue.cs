using System.Collections.Concurrent;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Webinex.Chatify.Common.Events;
using Webinex.Chatify.DataAccess;
using Webinex.Chatify.Types;
using Webinex.Chatify.Types.Events;

namespace Webinex.Chatify.Tasks;

internal interface IChatifyQueue
{
    Task EnqueueAsync(IEnumerable<AddMemberTask> tasks);
    Task EnqueueAsync(IEnumerable<RemoveMemberTask> tasks);
}

internal class ChatifyQueue : BackgroundService, IChatifyQueue
{
    private readonly ConcurrentQueue<object> _tasks = new();
    private CancellationToken? _cancellationToken;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<ChatifyQueue> _logger;

    public ChatifyQueue(IServiceProvider serviceProvider, ILogger<ChatifyQueue> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override Task ExecuteAsync(CancellationToken cancellationToken)
    {
        _cancellationToken = cancellationToken;

        Task.Run(async () =>
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                if (!_tasks.TryDequeue(out var result))
                {
                    await Task.Delay(50);
                    continue;
                }

                switch (result)
                {
                    case AddMemberTask addMemberTask:
                    {
                        await ExecuteAsync(addMemberTask);
                        break;
                    }

                    case RemoveMemberTask removeMemberTask:
                        await ExecuteAsync(removeMemberTask);
                        break;
                }
            }
        });

        return Task.CompletedTask;
    }

    private async Task ExecuteAsync(AddMemberTask task)
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var provider = scope.ServiceProvider;

            var eventService = provider.GetRequiredService<IEventService>();
            var dbContext = provider.GetRequiredService<ChatifyDbContext>();
            await using var transaction = await dbContext.Database.BeginTransactionAsync();
            dbContext.Members.Add(Member.New(task.ChatId, task.AccountId, task.AddedById, task.AddedAt));
            await dbContext.SaveChangesAsync();

            await dbContext.Database.ExecuteSqlRawAsync(
                @"
INSERT INTO [chatify].[Deliveries](ChatId, MessageId, ToId, FromId, [Read])
SELECT m.ChatId, m.Id, @p0, m.AuthorId, 1
    FROM [chatify].[Messages] m
    LEFT OUTER JOIN [chatify].[Deliveries] d on m.Id = d.MessageId and d.ToId = @p0
WHERE d.ToId is null and m.ChatId = @p1", task.AccountId, task.ChatId);

            var lastMessage = await dbContext.Messages.OrderByDescending(x => x.SentAt)
                .Where(x => x.ChatId == task.ChatId)
                .FirstAsync();

            await dbContext.ChatActivities.AddAsync(new ChatActivity(lastMessage.ChatId, task.AccountId,
                lastMessage.AuthorId, lastMessage.Id));
            await dbContext.SaveChangesAsync();

            eventService.Push(new MemberAddedEvent(task.ChatId, task.AccountId));
            await eventService.FlushAsync();
            await transaction.CommitAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception");
        }
    }

    private async Task ExecuteAsync(RemoveMemberTask task)
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var provider = scope.ServiceProvider;

            var eventService = provider.GetRequiredService<IEventService>();
            var dbContext = provider.GetRequiredService<ChatifyDbContext>();

            var member = await dbContext.Members.FindAsync(task.ChatId, task.AccountId);
            var chatActivity = await dbContext.ChatActivities.FindAsync(member!.ChatId, member.AccountId);

            await using var transaction = await dbContext.Database.BeginTransactionAsync();
            dbContext.Members.Remove(member!);
            dbContext.ChatActivities.Remove(chatActivity!);
            await dbContext.Deliveries.Where(x => x.ChatId == member!.ChatId && x.ToId == member.AccountId)
                .ExecuteDeleteAsync();
            await dbContext.SaveChangesAsync();

            eventService.Push(new MemberRemovedEvent(task.ChatId, task.AccountId, true));
            await eventService.FlushAsync();
            await transaction.CommitAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception");
        }
    }

    public Task EnqueueAsync(IEnumerable<AddMemberTask> tasks)
    {
        return EnqueueAsync(tasks.Cast<object>().ToArray());
    }

    public Task EnqueueAsync(IEnumerable<RemoveMemberTask> tasks)
    {
        return EnqueueAsync(tasks.Cast<object>().ToArray());
    }

    private Task EnqueueAsync(IEnumerable<object> tasks)
    {
        _cancellationToken?.ThrowIfCancellationRequested();
        tasks = tasks.ToArray();
        foreach (var task in tasks)
            _tasks.Enqueue(task);
        return Task.CompletedTask;
    }
}