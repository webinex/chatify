using Webinex.Chatify.Abstractions;
using Webinex.Chatify.Common.Events;
using Webinex.Chatify.Types.Events;

namespace Webinex.Chatify.Types;

public class Chat
{
    public Guid Id { get; protected init; }
    public string Name { get; protected set; } = null!;
    public DateTimeOffset CreatedAt { get; protected init; }
    public string CreatedById { get; protected init; } = null!;

    protected Chat()
    {
    }

    public static Chat New(
        IEventService eventService,
        AccountContext createdBy,
        string name,
        MessageContent? message,
        Guid? id = null,
        DateTimeOffset? now = null,
        IEnumerable<string>? members = null,
        string? requestId = null)
    {
        members = members?.ToArray();
        createdBy = createdBy ?? throw new ArgumentNullException(nameof(createdBy));

        var chat = new Chat
        {
            Id = id ?? Guid.NewGuid(),
            Name = name ?? throw new ArgumentNullException(nameof(name)),
            CreatedById = createdBy.Id,
            CreatedAt = now ?? DateTimeOffset.UtcNow,
        };

        eventService.Push(new ChatCreatedEvent(
            chat.Id,
            chat.Name,
            createdBy,
            chat.CreatedAt,
            members,
            Message: message != null
                ? new NewMessage(message.Text, message.Files, createdBy.Id)
                : NewMessage.ChatCreated(),
            requestId));
        return chat;
    }

    public void UpdateName(string name)
    {
        name = name ?? throw new ArgumentNullException(nameof(name));
        Name = name;
    }
}