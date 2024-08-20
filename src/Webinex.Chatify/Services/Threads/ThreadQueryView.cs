using Webinex.Chatify.Abstractions;
using Webinex.Chatify.DataAccess;
using Webinex.Chatify.Rows;
using Webinex.Chatify.Rows.Threads;
using Thread = Webinex.Chatify.Abstractions.Thread;

namespace Webinex.Chatify.Services.Threads;

internal class ThreadQueryView
{
    public required ThreadRow Thread { get; init; }
    public required ThreadWatchRow? Watch { get; init; }
    public required ThreadMessageRow? LastMessage { get; init; }
    public required AccountRow? LastMessageSentBy { get; init; }

    public static IQueryable<ThreadQueryView> Queryable(
        ChatifyDataConnection connection,
        AccountContext onBehalfOf,
        Thread.Props props,
        bool watchRequired = false)
    {
        if (props.HasFlag(Abstractions.Thread.Props.Watch) && onBehalfOf.IsSystem())
            throw new InvalidOperationException("Unable to request watch on behalf of system");

        var queryable = connection.ThreadRows.Select(thread => new ThreadQueryView
        {
            Thread = thread,
            Watch = null,
            LastMessage = null,
            LastMessageSentBy = null,
        });

        if (props.HasFlag(Abstractions.Thread.Props.Watch) && !watchRequired)
            queryable = queryable.GroupJoin(
                    connection.ThreadWatchRows,
                    view => new { ThreadId = view.Thread.Id, AccountId = onBehalfOf.Id },
                    watch => new { watch.ThreadId, watch.AccountId },
                    (view, watches) => new { view, watches })
                .SelectMany(
                    (join) => join.watches.DefaultIfEmpty(),
                    (join, watch) => new ThreadQueryView
                    {
                        Thread = join.view.Thread,
                        Watch = watch,
                        LastMessage = join.view.LastMessage,
                        LastMessageSentBy = join.view.LastMessageSentBy
                    });

        if (watchRequired)
            queryable = queryable.Join(
                connection.ThreadWatchRows,
                view => new { ThreadId = view.Thread.Id, AccountId = onBehalfOf.Id },
                watch => new { watch.ThreadId, watch.AccountId },
                (view, watch) => new ThreadQueryView
                {
                    Thread = view.Thread,
                    Watch = watch,
                    LastMessage = view.LastMessage,
                    LastMessageSentBy = view.LastMessageSentBy
                });

        if (props.HasFlag(Abstractions.Thread.Props.LastMessage))
            queryable = queryable.GroupJoin(
                    connection.ThreadMessageRows,
                    view => new { MessageId = view.Thread.LastMessageId! },
                    message => new { MessageId = message.Id },
                    (view, messages) => new { view, messages })
                .SelectMany((join) => join.messages.DefaultIfEmpty(),
                    (join, message) => new ThreadQueryView
                    {
                        Thread = join.view.Thread,
                        Watch = join.view.Watch,
                        LastMessage = message,
                        LastMessageSentBy = null,
                    });

        if (props.HasFlag(Abstractions.Thread.Props.LastMessageSentBy))
            queryable = queryable.GroupJoin(
                    connection.AccountRows,
                    view => new { AccountId = view.Thread.LastMessageId! },
                    account => new { AccountId = account.Id },
                    (view, accounts) => new { view, accounts })
                .SelectMany((join) => join.accounts.DefaultIfEmpty(),
                    (join, account) => new ThreadQueryView
                    {
                        Thread = join.view.Thread,
                        Watch = join.view.Watch,
                        LastMessage = join.view.LastMessage,
                        LastMessageSentBy = account,
                    });

        return queryable;
    }
}
