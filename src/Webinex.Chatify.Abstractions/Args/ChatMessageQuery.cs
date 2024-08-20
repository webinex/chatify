using Webinex.Asky;

namespace Webinex.Chatify.Abstractions;

public class ChatMessageQuery
{
    public ChatMessageQuery(
        AccountContext onBehalfOf,
        FilterRule? filterRule = null,
        SortRule[]? sortRule = null,
        PagingRule? pagingRule = null,
        ChatMessage.Props props = ChatMessage.Props.Default)
    {
        if (onBehalfOf.IsSystem())
            throw new ArgumentException("Message query on behalf of system is not supported", nameof(onBehalfOf));
        
        OnBehalfOf = onBehalfOf;
        FilterRule = filterRule;
        SortRule = sortRule;
        PagingRule = pagingRule;
        Props = props;
    }

    public AccountContext OnBehalfOf { get; }
    public FilterRule? FilterRule { get; }
    public SortRule[]? SortRule { get; }
    public PagingRule? PagingRule { get; }
    public ChatMessage.Props Props { get; }
}