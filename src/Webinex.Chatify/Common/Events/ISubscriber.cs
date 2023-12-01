namespace Webinex.Chatify.Common.Events;

public interface ISubscriber<T>
{
    Task InvokeAsync(T events);
}