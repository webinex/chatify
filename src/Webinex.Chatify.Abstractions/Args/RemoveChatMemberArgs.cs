namespace Webinex.Chatify.Abstractions;

public class RemoveChatMemberArgs
{
    public Guid ChatId { get; }
    public string AccountId { get; }
    public AccountContext OnBehalfOf { get; }
    public bool DeleteHistory { get; }

    public RemoveChatMemberArgs(Guid chatId, string accountId, AccountContext onBehalfOf, bool deleteHistory)
    {
        ChatId = chatId;
        AccountId = accountId ?? throw new ArgumentNullException(nameof(accountId));
        OnBehalfOf = onBehalfOf ?? throw new ArgumentNullException(nameof(onBehalfOf));
        DeleteHistory = deleteHistory;
    }
}