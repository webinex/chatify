namespace Webinex.Chatify.Abstractions;

public class ReadThreadMessageArgs
{
    public string MessageId { get; }
    public AccountContext OnBehalfOf { get; }

    public string ThreadId => ThreadMessageId.Parse(MessageId).ThreadId;
    
    public ReadThreadMessageArgs(string messageId, AccountContext onBehalfOf)
    {
        onBehalfOf = onBehalfOf ?? throw new ArgumentNullException(nameof(onBehalfOf));
        
        if (onBehalfOf.IsSystem())
            throw new InvalidOperationException("Cannot read thread message on behalf of system");

        MessageId = messageId ?? throw new ArgumentNullException(nameof(messageId));
        OnBehalfOf = onBehalfOf;
    }
}
