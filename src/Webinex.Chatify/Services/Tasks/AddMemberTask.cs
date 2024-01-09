namespace Webinex.Chatify.Services.Tasks;

internal class AddMemberTask : ITask
{
    public AddMemberTask(Guid chatId, string accountId, string addedById, DateTimeOffset addedAt)
    {
        ChatId = chatId;
        AccountId = accountId;
        AddedAt = addedAt;
        AddedById = addedById;
    }

    public Guid ChatId { get; }
    public string AccountId { get; }
    public string AddedById { get; }
    public DateTimeOffset AddedAt { get; }
}