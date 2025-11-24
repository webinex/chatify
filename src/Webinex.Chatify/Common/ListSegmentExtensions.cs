using Webinex.Chatify.Abstractions;

namespace Webinex.Chatify.Common;

internal static class ListSegmentExtensions
{
    public static ListSegment<TResult> Map<TSource, TResult>(
        this ListSegment<TSource> source,
        Func<TSource, TResult> map)
    {
        return new ListSegment<TResult>(source.Items.Select(map), source.Total);
    }
}
