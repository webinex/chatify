using Webinex.Chatify.Abstractions;
using File = Webinex.Chatify.Abstractions.File;

namespace Webinex.Chatify.AspNetCore;

public class ChatMessageDto
{
    public string Id { get; }
    public Guid ChatId { get; }
    public string? Text { get; }
    public DateTimeOffset SentAt { get; }
    public IReadOnlyCollection<File> Files { get; }
    public AccountDto SentBy { get; }

    public ChatMessageDto(
        string id,
        Guid chatId,
        string? text,
        DateTimeOffset sentAt,
        IEnumerable<File> files,
        AccountDto sentBy)
    {
        Id = id;
        ChatId = chatId;
        Text = text;
        SentAt = sentAt;
        Files = files.ToArray();
        SentBy = sentBy;
    }

    public ChatMessageDto(ChatMessage chatMessage)
    {
        Id = chatMessage.Id;
        ChatId = chatMessage.ChatId;
        Text = chatMessage.Body.Text;
        SentAt = chatMessage.SentAt;
        Files = chatMessage.Body.Files;
        SentBy = new AccountDto(chatMessage.Author.Value!);
    }
}
