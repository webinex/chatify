using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Webinex.Chatify.Abstractions.Events;

namespace Webinex.Chatify.Common;

internal interface IEventService
{
    void Push<T>(T @event) where T : class;
    Task FlushAsync();
}

internal class EventService : IEventService
{
    private readonly List<object> _events = new();
    private readonly List<object> _history = new();
    private readonly object _lock = new();
    private readonly IServiceProvider _serviceProvider;

    private static readonly MethodInfo INVOKE_METHOD =
        typeof(EventService).GetMethod(nameof(InvokeEventAsync), BindingFlags.Instance | BindingFlags.NonPublic)!;

    public EventService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public void Push<T>(T @event)
        where T : class
    {
        lock (_lock)
        {
            _events.Add(@event);
            _history.Add(@event);
        }
    }

    public async Task FlushAsync()
    {
        object[] events;

        lock (_lock)
        {
            events = _events.ToArray();
            _events.Clear();
        }

        foreach (var @event in events)
        {
            await InvokeAsync(@event, events);
        }

        if (_events.Any())
            await FlushAsync();
    }

    private async Task InvokeAsync(object @event, object[] events)
    {
        var method = INVOKE_METHOD.MakeGenericMethod(@event.GetType());
        var task = (Task)method.Invoke(this, new[] { @event, events })!;
        await task;
    }

    private async Task InvokeEventAsync<T>(T @event, object[] events)
        where T : class
    {
        await InvokeSubscribersAsync(@event);

        var ofType = events.OfType<T>().ToArray();
        var lastOfType = ofType.Last() == @event;
        if (lastOfType)
            await InvokeCollectionSubscribersAsync(ofType);
    }

    private async Task InvokeSubscribersAsync<T>(T @event)
        where T : class
    {
        var subscribers = _serviceProvider.GetServices<IEventSubscriber<T>>();

        foreach (var subscriber in subscribers)
        {
            await subscriber.InvokeAsync(@event);
        }
    }

    private async Task InvokeCollectionSubscribersAsync<T>(T[] events)
        where T : class
    {
        var subscribers = _serviceProvider.GetServices<IEventSubscriber<IEnumerable<T>>>().ToArray();
        subscribers = subscribers.OrderBy(x =>
                x.GetType().GetCustomAttribute<EventSubscriberPriorityAttribute>()?.Priority ?? int.MaxValue)
            .ThenBy(x => Array.IndexOf(subscribers, x))
            .ToArray();

        foreach (var subscriber in subscribers)
        {
            await subscriber.InvokeAsync(events);
        }
    }
}
