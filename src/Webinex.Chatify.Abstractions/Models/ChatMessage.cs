using System.Text.Json;

namespace Webinex.Chatify.Abstractions;

public class ChatMessage
{
    public string Id { get; }
    public Guid ChatId { get; }
    public string AuthorId { get; }
    public DateTimeOffset SentAt { get; }
    public MessageBody Body { get; }
    public Optional<Account> Author { get; }
    public Optional<bool> Read { get; }

    public ChatMessage(
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
    
    public static class WellKnown
    {
        public static readonly string MemberRemoved = "chatify://member-removed";
        public static readonly string MemberAdded = "chatify://member-added";
        public static readonly string ChatCreated = "chatify://chat-created";

        public static string ChatRenamed(string newName)
        {
            return
                $"chatify://chat-name-changed::{JsonSerializer.Serialize(new { NewName = newName }, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase })}";
        }
    }
}

public static class MessagePropExtensions
{
    public static bool HasDeliveryProps(this ChatMessage.Props props)
    {
        return props.HasFlag(ChatMessage.Props.Read);
    }
}
