namespace Webinex.Chatify.Rows.Chats;

internal class ChatMetaRow
{
    public Guid ChatId { get; protected init; }
    public int LastIndex { get; protected set; }

    protected ChatMetaRow()
    {
    }

    public void Increment()
    {
        LastIndex++;
    }

    public static ChatMetaRow New(Guid chatId, int index = 0)
    {
        return new ChatMetaRow
        {
            ChatId = chatId,
            LastIndex = 0,
        };
    }
}
