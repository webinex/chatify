namespace Webinex.Chatify.Abstractions;

public class UpdateChatNameArgs
{
    public UpdateChatNameArgs(Guid id, string name, AccountContext onBehalfOf)
    {
        Id = id;
        Name = name ?? throw new ArgumentNullException(nameof(name));
        OnBehalfOf = onBehalfOf ?? throw new ArgumentNullException(nameof(onBehalfOf));
    }

    public Guid Id { get; }
    public string Name { get; }
    public AccountContext OnBehalfOf { get; }
}
