using Webinex.Asky;

namespace Webinex.Chatify.Abstractions;

public class ChatQuery
{
    public FilterRule? FilterRule { get; }
    public SortRule[]? SortRule { get; }
    public AccountContext OnBehalfOf { get; }
    public Chat.Props Props { get; }

    public ChatQuery(
        AccountContext onBehalfOf,
        FilterRule? filterRule = null,
        SortRule[]? sortRule = null,
        Chat.Props props = Chat.Props.Default)
    {
        if (onBehalfOf.IsSystem())
            throw new ArgumentException("Chat query on behalf of system is not supported", nameof(onBehalfOf));
        
        FilterRule = filterRule;
        SortRule = sortRule;
        OnBehalfOf = onBehalfOf;
        Props = props;
    }
}

