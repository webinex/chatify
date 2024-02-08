namespace Webinex.Chatify.Rows;

internal class ChatMetaRow
{
    public Guid ChatId { get; protected init; }
    public int LastIndex { get; protected set; }

    protected ChatMetaRow()
    {
    }

    public static string FormatSqlIncrement((string ChatIdRef, string ResultIdRef) args)
    {
        return $"""
                update chatify.ChatMeta with (rowlock, updlock, holdlock)
                set [LastIndex] = [LastIndex] + 1, {args.ResultIdRef} = [LastIndex] + 1
                where ChatId = {args.ChatIdRef}
                """;
    }
}
