namespace Webinex.Chatify.Abstractions;

public class Message
{
    public string Id { get; }
    public Guid ChatId { get; }
    public string AuthorId { get; }
    public DateTimeOffset SentAt { get; }
    public MessageBody Body { get; }
    public Optional<Account> Author { get; }
    public Optional<bool> Read { get; }

    public Message(
        string id,
        Guid chatId,
        string authorId,
        DateTimeOffset sentAt,
        MessageBody body,
        Optional<bool> read,
        Optional<Account> author)
    {
        Id = id;
        ChatId = chatId;
        AuthorId = authorId;
        SentAt = sentAt;
        Body = body;
        Author = author;
        Read = read;
    }

    [Flags]
    public enum Props
    {
        Default = 0,
        Author = 1,
        Read = 2,
    }
}

public static class MessagePropExtensions
{
    public static bool HasDeliveryProps(this Message.Props props)
    {
        return props.HasFlag(Message.Props.Read);
    }
}
