using Webinex.Asky;

namespace Webinex.Chatify.Abstractions;

public class ChatQuery
{
    public ChatQuery(
        AccountContext onBehalfOf,
        FilterRule? filterRule = null,
        SortRule[]? sortRule = null,
        PagingRule? pagingRule = null,
        ChatQueryProp prop = ChatQueryProp.Unspecified)
    {
        FilterRule = filterRule;
        SortRule = sortRule;
        PagingRule = pagingRule;
        OnBehalfOf = onBehalfOf;
        Prop = prop;
    }

    public FilterRule? FilterRule { get; }
    public SortRule[]? SortRule { get; }
    public PagingRule? PagingRule { get; }
    public AccountContext OnBehalfOf { get; }
    public ChatQueryProp Prop { get; }
}