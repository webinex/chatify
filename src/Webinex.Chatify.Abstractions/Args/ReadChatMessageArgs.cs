namespace Webinex.Chatify.Abstractions;

public class ReadChatMessageArgs
{
    public string Id { get; }
    public AccountContext OnBehalfOf { get; }

    public ReadChatMessageArgs(string messageId, AccountContext onBehalfOf)
    {
        if (onBehalfOf.IsSystem())
            throw new ArgumentException("System account is not allowed to read messages", nameof(onBehalfOf));
        
        Id = messageId ?? throw new ArgumentNullException(nameof(messageId));
        OnBehalfOf = onBehalfOf ?? throw new ArgumentNullException(nameof(onBehalfOf));
    }
}
