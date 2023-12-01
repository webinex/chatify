namespace Webinex.Chatify.Abstractions;

public class ReadCommand
{
    public ReadCommand(IReadOnlyCollection<string> ids, AccountContext onBehalfOf)
    {
        Ids = ids;
        OnBehalfOf = onBehalfOf;
    }

    public IReadOnlyCollection<string> Ids { get; }
    public AccountContext OnBehalfOf { get; }
}