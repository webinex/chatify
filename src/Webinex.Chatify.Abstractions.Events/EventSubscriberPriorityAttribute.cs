namespace Webinex.Chatify.Abstractions.Events;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class EventSubscriberPriorityAttribute : Attribute
{
    public int Priority { get; }
    
    public EventSubscriberPriorityAttribute(int priority)
    {
        Priority = priority;
    }
}
