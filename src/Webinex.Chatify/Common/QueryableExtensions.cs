using LinqToDB;
using Webinex.Asky;
using Webinex.Chatify.Abstractions;

namespace Webinex.Chatify.Common;

internal static class QueryableExtensions
{
    public static async Task<ListSegment<T>> ToListSegmentAsync<T>(
        this IQueryable<T> queryable,
        PagingRule? pagingRule = null,
        bool includeTotal = false)
    {
        if (pagingRule == null)
        {
            var result = await queryable.ToArrayAsync();
            return new ListSegment<T>(result, includeTotal ? result.Length : -1);
        }

        var pagedQueryable = queryable.PageBy(pagingRule);
        var items = await pagedQueryable.ToArrayAsync();
        var total = includeTotal ? await queryable.CountAsync() : -1;
        return new ListSegment<T>(items, total);
    }
}
