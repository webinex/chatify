using Webinex.Chatify.Abstractions;
using Webinex.Chatify.Rows;
using Webinex.Chatify.Rows.Threads;
using Thread = Webinex.Chatify.Abstractions.Thread;

namespace Webinex.Chatify.Services.Threads;

internal static class ThreadMessageMapper
{
    private record Input(
        ThreadMessageRow Message,
        ThreadRow? Thread,
        ThreadWatchRow? Watch,
        AccountRow? SentBy,
        ThreadMessage.Props Props);

    public static ThreadMessage Map(ThreadMessageQueryView view, ThreadMessage.Props props)
    {
        return Map(new Input(view.Message, view.Thread, view.Watch, view.SentBy, props));
    }

    public static ThreadMessage Map(
        ThreadMessageRow message,
        ThreadRow? thread = null,
        ThreadWatchRow? watch = null,
        AccountRow? sentBy = null,
        ThreadMessage.Props props = ThreadMessage.Props.Default)
    {
        return Map(new Input(message, thread, watch, sentBy, props));
    }

    private static ThreadMessage Map(Input input)
    {
        var sentBy = !input.Props.HasFlag(ThreadMessage.Props.SentBy)
            ? Optional.NoValue<Account>()
            : Optional.Value(input.SentBy!.ToAbstraction());

        var read = !input.Props.HasFlag(ThreadMessage.Props.Read)
            ? Optional.NoValue<bool?>()
            : Optional.Value<bool?>(string.Compare(input.Message.Id, input.Watch!.LastReadMessageId,
                StringComparison.InvariantCultureIgnoreCase) <= 0);

        var thread = !input.Props.HasFlag(ThreadMessage.Props.Thread)
            ? Optional.NoValue<Thread>()
            : Optional.Value(ThreadMapper.Map(input.Thread!));

        var value = new ThreadMessage(input.Message.Id, input.Message.ThreadId, input.Message.SentById,
            input.Message.SentAt, input.Message.Text, input.Message.Files, read, sentBy, thread);

        return value;
    }
}
