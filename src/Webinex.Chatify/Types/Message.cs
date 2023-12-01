using Webinex.Chatify.Abstractions;
using Webinex.Chatify.Common.Events;
using Webinex.Chatify.Types.Events;

namespace Webinex.Chatify.Types;

public class Message
{
    public string Id { get; protected init; } = null!;
    public Guid ChatId { get; protected init; }
    public string Text { get; protected init; } = null!;
    public string AuthorId { get; protected init; } = null!;
    public DateTimeOffset SentAt { get; protected init; }
    public IReadOnlyCollection<File> Files { get; protected init; } = null!;

    internal Message(string id, Guid chatId, string authorId, DateTimeOffset sentAt, string text, IReadOnlyCollection<File> files)
    {
        Id = id;
        ChatId = chatId;
        AuthorId = authorId;
        SentAt = sentAt;
        Text = text ?? throw new ArgumentNullException(nameof(text));
        Files = files ?? throw new ArgumentNullException(nameof(files));
    }

    protected Message()
    {
    }

    internal static Message New(
        IEventService eventService,
        Guid chatId,
        string authorId,
        string text,
        IEnumerable<File> files,
        IEnumerable<string> recipients,
        ChatCreatedEvent? chatCreatedEvent,
        string? requestId = null)
    {
        files = files.ToArray();
        recipients = recipients ?? throw new ArgumentNullException(nameof(recipients));

        var now = DateTimeOffset.UtcNow;
        var id = new MessageId(now);
        
        var message = new Message(id.ToString(), chatId, authorId, DateTimeOffset.UtcNow, text, files.ToArray());
        eventService.Push(new MessageCreatedEvent(message.Id, message.ChatId, new MessageContent(text, files.ToList()),
            authorId, recipients, message.SentAt, chatCreatedEvent, requestId));
        return message;
    }
}