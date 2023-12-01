namespace Webinex.Chatify.Abstractions;

public class RemoveMemberCommand
{
    public Guid ChatId { get; }
    public string AccountId { get; }
    public AccountContext OnBehalfOf { get; }
    public bool DeleteHistory { get; }

    public RemoveMemberCommand(Guid chatId, string accountId, AccountContext onBehalfOf, bool deleteHistory)
    {
        ChatId = chatId;
        AccountId = accountId;
        OnBehalfOf = onBehalfOf;
        DeleteHistory = deleteHistory;
    }
}