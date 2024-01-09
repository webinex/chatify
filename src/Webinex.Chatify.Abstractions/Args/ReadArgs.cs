namespace Webinex.Chatify.Abstractions;

public class ReadArgs
{
    public IReadOnlyCollection<string> MessageIds { get; }
    public AccountContext OnBehalfOf { get; }

    public ReadArgs(IReadOnlyCollection<string> ids, AccountContext onBehalfOf)
    {
        if (onBehalfOf.IsSystem())
            throw new ArgumentException("System account is not allowed to read messages", nameof(onBehalfOf));
        
        MessageIds = ids?.Distinct().ToArray() ?? throw new ArgumentNullException(nameof(ids));
        OnBehalfOf = onBehalfOf ?? throw new ArgumentNullException(nameof(onBehalfOf));
    }
}
