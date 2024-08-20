using Webinex.Chatify.Abstractions;
using Webinex.Chatify.Rows;
using Webinex.Chatify.Rows.Threads;
using Thread = Webinex.Chatify.Abstractions.Thread;

namespace Webinex.Chatify.Services.Threads;

internal static class ThreadMapper
{
    private record Input(
        ThreadRow Thread,
        ThreadWatchRow? Watch,
        ThreadMessageRow? LastMessage,
        AccountRow? LastMessageSentBy,
        Thread.Props Props);

    public static Thread Map(ThreadQueryView view, Thread.Props props)
    {
        return Map(new Input(view.Thread, view.Watch, view.LastMessage, view.LastMessageSentBy, props));
    }

    public static Thread Map(
        ThreadRow thread,
        ThreadWatchRow? watch = null,
        ThreadMessageRow? lastMessage = null,
        AccountRow? lastMessageSentBy = null,
        Thread.Props props = Thread.Props.Default)
    {
        return Map(new Input(thread, watch, lastMessage, lastMessageSentBy, props));
    }

    private static Thread Map(Input input)
    {
        var thread = new Thread(
            input.Thread.Id,
            input.Thread.Name,
            input.Thread.CreatedById,
            input.Thread.CreatedAt,
            input.Thread.Archived,
            input.Thread.LastMessageId,
            MapThreadLastMessage(input),
            MapThreadWatch(input),
            MapThreadLastReadMessageId(input));

        return thread;
    }

    private static Optional<bool> MapThreadWatch(Input input)
    {
        if (!input.Props.HasFlag(Thread.Props.Watch))
            return Optional.NoValue<bool>();

        return Optional.Value(input.Watch != null);
    }

    private static Optional<ThreadMessage?> MapThreadLastMessage(Input input)
    {
        if (!input.Props.HasFlag(Thread.Props.LastMessage))
            return Optional.NoValue<ThreadMessage?>();

        if (input.LastMessage == null)
            return Optional.Value<ThreadMessage?>(null);

        var value = ThreadMessageMapper.Map(
            input.LastMessage,
            watch: input.Watch,
            sentBy: input.LastMessageSentBy,
            props: input.Props.ToLastMessageProps());

        return Optional.Value<ThreadMessage?>(value);
    }

    private static Optional<string?> MapThreadLastReadMessageId(Input input)
    {
        if (!input.Props.HasFlag(Thread.Props.Watch))
            return Optional.NoValue<string?>();

        if (input.Watch == null)
            return Optional.NoValue<string?>();

        return Optional.Value<string?>(input.Watch!.LastReadMessageId);
    }
}
