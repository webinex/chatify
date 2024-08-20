using Webinex.Asky;

namespace Webinex.Chatify.Abstractions;

public class ThreadMessageQuery
{
    public ThreadMessageQuery(
        AccountContext onBehalfOf,
        string threadId,
        bool sentAtAsc,
        PagingRule pagingRule,
        ThreadMessage.Props props = ThreadMessage.Props.Default)
    {
        if (onBehalfOf.IsSystem())
            throw new ArgumentException("Thread message query on behalf of system is not supported", nameof(onBehalfOf));
        
        OnBehalfOf = onBehalfOf;
        ThreadId = threadId;
        SentAtAsc = sentAtAsc;
        PagingRule = pagingRule;
        Props = props;
    }

    public AccountContext OnBehalfOf { get; }
    public string ThreadId { get; }
    public bool SentAtAsc { get; }
    public PagingRule PagingRule { get; }
    public ThreadMessage.Props Props { get; }
}
