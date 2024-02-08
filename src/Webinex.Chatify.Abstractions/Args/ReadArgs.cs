namespace Webinex.Chatify.Abstractions;

public class ReadArgs
{
    public string Id { get; }
    public AccountContext OnBehalfOf { get; }

    public ReadArgs(string messageId, AccountContext onBehalfOf)
    {
        if (onBehalfOf.IsSystem())
            throw new ArgumentException("System account is not allowed to read messages", nameof(onBehalfOf));
        
        Id = messageId ?? throw new ArgumentNullException(nameof(messageId));
        OnBehalfOf = onBehalfOf ?? throw new ArgumentNullException(nameof(onBehalfOf));
    }
}
