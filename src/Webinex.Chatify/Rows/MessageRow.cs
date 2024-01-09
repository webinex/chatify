using System.Text.Json;
using Webinex.Chatify.Abstractions;
using Webinex.Chatify.Abstractions.Events;
using Webinex.Chatify.Common;
using File = Webinex.Chatify.Abstractions.File;

namespace Webinex.Chatify.Rows;

internal class MessageRow
{
    public string Id { get; protected init; } = null!;
    public Guid ChatId { get; protected init; }
    public string Text { get; protected init; } = null!;
    public string AuthorId { get; protected init; } = null!;
    public DateTimeOffset SentAt { get; protected init; }
    public IReadOnlyCollection<File> Files { get; protected init; } = null!;
    public virtual AccountRow Author { get; protected init; } = null!;

    internal MessageRow(
        string id,
        Guid chatId,
        string authorId,
        DateTimeOffset sentAt,
        string text,
        IReadOnlyCollection<File> files)
    {
        Id = id;
        ChatId = chatId;
        AuthorId = authorId;
        SentAt = sentAt;
        Text = text ?? throw new ArgumentNullException(nameof(text));
        Files = files ?? throw new ArgumentNullException(nameof(files));
    }

    protected MessageRow()
    {
    }

    internal static MessageRow NewSent(
        IEventService eventService,
        Guid chatId,
        string authorId,
        MessageBody body,
        string? requestId = null)
    {
        var message = new MessageRow(
            MessageId.New().ToString(),
            chatId,
            authorId,
            DateTimeOffset.UtcNow,
            body.Text,
            body.Files.ToArray());

        eventService.Push(new MessageSentEvent(
            message.Id,
            message.ChatId,
            body,
            authorId,
            message.SentAt,
            requestId));

        return message;
    }

    internal static MessageRow NewChatCreated(
        IEventService eventService,
        Guid chatId,
        string chatName,
        IEnumerable<string> chatMembers,
        string authorId,
        MessageBody body,
        string? requestId = null)
    {
        var message = new MessageRow(
            MessageId.New().ToString(),
            chatId,
            authorId,
            DateTimeOffset.UtcNow,
            body.Text,
            body.Files.ToArray());

        eventService.Push(new NewChatMessageCreatedEvent(
            message.Id,
            new NewChatMessageCreatedEvent.ChatValue(chatId, chatName, chatMembers.ToArray()),
            body,
            authorId,
            message.SentAt,
            requestId));

        return message;
    }

    internal static MessageRow NewMemberAddedByJob(Guid chatId, string addedById, string memberId)
    {
        return new MessageRow(MessageId.New().ToString(), chatId, AccountContext.System.Id, DateTimeOffset.UtcNow,
            $"chatify://member-added::{JsonSerializer.Serialize(new { addedById, memberId })}", Array.Empty<File>());
    }
}
