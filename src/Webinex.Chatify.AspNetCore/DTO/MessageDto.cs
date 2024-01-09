using Webinex.Chatify.Abstractions;
using File = Webinex.Chatify.Abstractions.File;

namespace Webinex.Chatify.AspNetCore;

public class MessageDto
{
    public string Id { get; }
    public Guid ChatId { get; }
    public string Text { get; }
    public DateTimeOffset SentAt { get; }
    public IReadOnlyCollection<File> Files { get; }
    public AccountDto SentBy { get; }
    public bool Read { get; }

    public MessageDto(
        string id,
        Guid chatId,
        string text,
        DateTimeOffset sentAt,
        IEnumerable<File> files,
        AccountDto sentBy,
        bool read)
    {
        Id = id;
        ChatId = chatId;
        Text = text;
        SentAt = sentAt;
        Files = files.ToArray();
        SentBy = sentBy;
        Read = read;
    }

    public MessageDto(Message message)
    {
        Id = message.Id;
        ChatId = message.ChatId;
        Text = message.Body.Text;
        SentAt = message.SentAt;
        Files = message.Body.Files;
        SentBy = new AccountDto(message.Author.Value!);
        Read = message.Read.Value;
    }
}
