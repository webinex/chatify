using LinqToDB;
using Webinex.Chatify.Abstractions;
using Webinex.Chatify.DataAccess;
using Webinex.Chatify.Rows;
using Webinex.Chatify.Rows.Threads;

namespace Webinex.Chatify.Services.Threads;

internal class ThreadMessageQueryView
{
    public required ThreadMessageRow Message { get; init; }
    public required ThreadRow? Thread { get; init; }
    public required ThreadWatchRow? Watch { get; init; }
    public required AccountRow? SentBy { get; init; }

    public static IQueryable<ThreadMessageQueryView> Queryable(
        ChatifyDataConnection connection,
        ThreadMessage.Props props,
        AccountContext? onBehalfOf = null)
    {
        if (props.HasFlag(ThreadMessage.Props.Read) && (onBehalfOf == null || onBehalfOf.IsSystem()))
            throw new ArgumentException("Read messages require onBehalfOf not a system user", nameof(onBehalfOf));

        var queryable = connection.ThreadMessageRows.AsQueryable().Select(x => new ThreadMessageQueryView
        {
            Message = x,
            Thread = null,
            SentBy = null,
            Watch = null
        });

        if (props.HasFlag(ThreadMessage.Props.Thread))
            queryable = queryable.Join(connection.ThreadRows, view => view.Message.ThreadId, thread => thread.Id,
                (view, thread) => new ThreadMessageQueryView
                {
                    Message = view.Message,
                    Thread = thread,
                    SentBy = null,
                    Watch = null,
                });

        if (props.HasFlag(ThreadMessage.Props.SentBy))
            queryable = queryable.Join(connection.AccountRows, view => view.Message.SentById, account => account.Id,
                (view, account) => new ThreadMessageQueryView
                {
                    Message = view.Message,
                    Thread = view.Thread,
                    SentBy = account,
                    Watch = null,
                });

        if (props.HasFlag(ThreadMessage.Props.Read))
            queryable = queryable.LeftJoin(connection.ThreadWatchRows,
                (view, watch) => view.Message.ThreadId == watch.ThreadId && watch.AccountId == onBehalfOf!.Id,
                (view, watch) => new ThreadMessageQueryView
                {
                    Message = view.Message,
                    Thread = view.Thread,
                    SentBy = view.SentBy,
                    Watch = watch,
                });

        return queryable;
    }
}
