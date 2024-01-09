namespace Webinex.Chatify.Abstractions.Events;

public interface IEventSubscriber<T>
{
    Task InvokeAsync(T events);
}