namespace Webinex.Chatify.Abstractions;

public class ThreadMessageId : Equatable
{
    private static readonly string DELIMETER = "::";

    public string ThreadId { get; }
    public int Index { get; }

    public ThreadMessageId(string threadId, int index)
    {
        if (threadId.Contains(DELIMETER))
            throw new ArgumentException($"Cannot contain ${DELIMETER}", nameof(threadId));
        
        ThreadId = threadId;
        Index = index;
    }

    public static ThreadMessageId Parse(string value)
    {
        var parts = value.Split(DELIMETER);
        return new ThreadMessageId(parts[0], int.Parse(parts[1]));
    }

    public override string ToString()
    {
        return $"{ThreadId}{DELIMETER}{Index.ToString().PadLeft(9, '0')}";
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return ThreadId;
        yield return Index;
    }
}
