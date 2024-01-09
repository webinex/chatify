namespace Webinex.Chatify.AspNetCore;

public class ChatDto
{
    public ChatDto(Guid id, string name, AccountDto[] members)
    {
        Id = id;
        Name = name;
        Members = members;
    }

    public Guid Id { get; }
    public string Name { get; }
    public AccountDto[] Members { get; }
}