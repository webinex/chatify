namespace Webinex.Chatify.Abstractions;

public abstract class ChatMember
{
    public abstract Guid ChatId { get; }
    public abstract string AccountId { get; }
    public abstract string AddedById { get; }
    public abstract DateTimeOffset AddedAt { get; }    
}
