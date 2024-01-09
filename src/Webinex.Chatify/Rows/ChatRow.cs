using Webinex.Chatify.Abstractions;
using Webinex.Chatify.Abstractions.Events;
using Webinex.Chatify.Common;

namespace Webinex.Chatify.Rows;

internal class ChatRow
{
    public Guid Id { get; protected init; }
    public string Name { get; protected set; } = null!;
    public DateTimeOffset CreatedAt { get; protected init; }
    public string CreatedById { get; protected set; } = null!;

    protected ChatRow()
    {
    }

    public void UpdateName(string name)
    {
        name = name ?? throw new ArgumentNullException(nameof(name));
        Name = name;
    }

    public static ChatRow New(
        IEventService eventService,
        AccountContext createdBy,
        string name,
        IEnumerable<string> members,
        MessageBody? message,
        Guid? id = null,
        DateTimeOffset? now = null,
        string? requestId = null)
    {
        var memberArray = members.ToArray();
        createdBy = createdBy ?? throw new ArgumentNullException(nameof(createdBy));

        var chat = new ChatRow
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
            memberArray,
            Message: message,
            requestId));

        return chat;
    }
}
