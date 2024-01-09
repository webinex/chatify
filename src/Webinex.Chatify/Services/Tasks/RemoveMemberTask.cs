namespace Webinex.Chatify.Services.Tasks;

internal class RemoveMemberTask : ITask
{
    public RemoveMemberTask(Guid chatId, string accountId)
    {
        ChatId = chatId;
        AccountId = accountId;
    }

    public Guid ChatId { get; }
    public string AccountId { get; }
}