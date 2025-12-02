namespace Webinex.Chatify.Rows.Chats;

internal class ChatMetaRow
{
    public Guid ChatId { get; protected init; }
    public int LastIndex { get; protected set; }
    public string LastMessageId { get; protected set; } = null!;

    protected ChatMetaRow()
    {
    }

    public void Increment(string messageId)
    {
        LastIndex++;
        LastMessageId = messageId ?? throw new ArgumentNullException(nameof(messageId));
    }

    public static ChatMetaRow New(Guid chatId, string lastMessageId, int index = 0)
    {
        return new ChatMetaRow
        {
            ChatId = chatId,
            LastIndex = index,
            LastMessageId = lastMessageId,
        };
    }
}
