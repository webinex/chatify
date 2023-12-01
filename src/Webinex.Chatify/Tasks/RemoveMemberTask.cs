namespace Webinex.Chatify.Tasks;

public class RemoveMemberTask
{
    public RemoveMemberTask(Guid chatId, string accountId)
    {
        ChatId = chatId;
        AccountId = accountId;
    }

    public Guid ChatId { get; }
    public string AccountId { get; }
}