using System.Collections.Concurrent;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Webinex.Chatify.Services.Tasks;

namespace Webinex.Chatify;

internal interface IChatifyQueue
{
    Task EnqueueAsync(IEnumerable<ITask> tasks);
}

internal class ChatifyQueue : BackgroundService, IChatifyQueue
{
    private readonly ConcurrentQueue<ITask> _tasks = new();
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
                if (!_tasks.TryDequeue(out var task))
                {
                    await Task.Delay(50, cancellationToken);
                    continue;
                }

                typeof(ChatifyQueue).GetMethod(nameof(ExecuteTaskAsync), BindingFlags.NonPublic | BindingFlags.Instance)!
                    .MakeGenericMethod(task.GetType())
                    .Invoke(this, new object[] { task });
            }
        });

        return Task.CompletedTask;
    }

    private async Task ExecuteTaskAsync<TTask>(TTask task)
        where TTask : ITask
    {
        try
        {
            using var serviceScope = _serviceProvider.CreateScope();
            var provider = serviceScope.ServiceProvider;
            var job = provider.GetRequiredService<IJob<TTask>>();
            await job.InvokeAsync(task);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Task execution failed...");
        }
    }


    public Task EnqueueAsync(IEnumerable<ITask> tasks)
    {
        _cancellationToken?.ThrowIfCancellationRequested();
        tasks = tasks.ToArray();
        foreach (var task in tasks)
            _tasks.Enqueue(task);
        return Task.CompletedTask;
    }
}