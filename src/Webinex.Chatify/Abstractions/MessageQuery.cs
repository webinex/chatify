using Webinex.Asky;

namespace Webinex.Chatify.Abstractions;

public class MessageQuery
{
    public MessageQuery(
        AccountContext onBehalfOf,
        FilterRule? filterRule = null,
        SortRule[]? sortRule = null,
        PagingRule? pagingRule = null)
    {
        OnBehalfOf = onBehalfOf;
        FilterRule = filterRule;
        SortRule = sortRule;
        PagingRule = pagingRule;
    }

    public AccountContext OnBehalfOf { get; }
    public FilterRule? FilterRule { get; }
    public SortRule[]? SortRule { get; }
    public PagingRule? PagingRule { get; }
}