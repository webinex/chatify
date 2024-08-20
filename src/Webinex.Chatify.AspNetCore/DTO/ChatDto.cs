using Webinex.Chatify.Abstractions;

namespace Webinex.Chatify.AspNetCore;

public class ChatDto
{
    public Guid Id { get; }
    public string Name { get; }
    public AccountDto[] Members { get; }
    public bool Active { get; }
    public string? LastReadMessageId { get; }

    public ChatDto(Chat chat, AccountDto[] members)
    {
        Id = chat.Id;
        Name = chat.Name;
        Active = chat.Active.Value;
        LastReadMessageId = chat.LastReadMessageId.Value;
        Members = members;
    }
}
