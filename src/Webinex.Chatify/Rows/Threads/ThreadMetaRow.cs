namespace Webinex.Chatify.Rows.Threads;

public class ThreadMetaRow
{
    public string ThreadId { get; protected init; } = null!;
    public int? LastIndex { get; protected set; }

    public ThreadMetaRow(string threadId, int? lastIndex)
    {
        ThreadId = threadId;
        LastIndex = lastIndex;
    }

    protected ThreadMetaRow()
    {
    }

    public int Increment()
    {
        LastIndex = (LastIndex ?? -1) + 1;
        return LastIndex.Value;
    }
}
