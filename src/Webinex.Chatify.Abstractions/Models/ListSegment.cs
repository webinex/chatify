namespace Webinex.Chatify.Abstractions;

public class ListSegment<T>
{
    public IReadOnlyCollection<T> Items { get; }
    public int Total { get; }

    public ListSegment(IEnumerable<T> items, int total)
    {
        Items = items.ToArray();
        Total = total;
    }
}
