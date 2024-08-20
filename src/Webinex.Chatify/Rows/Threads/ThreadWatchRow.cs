namespace Webinex.Chatify.Rows.Threads;

internal class ThreadWatchRow
{
    public string ThreadId { get; protected init; } = null!;
    public string AccountId { get; protected init; } = null!;
    public string? LastReadMessageId { get; protected init; }
    
    protected ThreadWatchRow(string threadId, string accountId, string? lastReadMessageId = null)
    {
        ThreadId = threadId;
        AccountId = accountId;
        LastReadMessageId = lastReadMessageId;
    }

    protected ThreadWatchRow()
    {
    }
    
    public static ThreadWatchRow New(string threadId, string accountId, string? lastReadMessageId = null)
    {
        threadId = threadId ?? throw new ArgumentNullException(nameof(threadId));
        accountId = accountId ?? throw new ArgumentNullException(nameof(accountId));

        return new ThreadWatchRow(threadId, accountId, lastReadMessageId);
    }
}
