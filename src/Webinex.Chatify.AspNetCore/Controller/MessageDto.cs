using Webinex.Chatify.Types;
using File = Webinex.Chatify.Types.File;

namespace Webinex.Chatify.AspNetCore.Controller;

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

    public MessageDto(Message message, Delivery delivery, Account sentBy)
    {
        Id = message.Id;
        ChatId = message.ChatId;
        Text = message.Text;
        SentAt = message.SentAt;
        Files = message.Files;
        SentBy = new AccountDto(sentBy);
        Read = delivery.Read;
    }
}