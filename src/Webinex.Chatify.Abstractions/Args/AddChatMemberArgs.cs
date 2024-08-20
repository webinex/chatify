namespace Webinex.Chatify.Abstractions;

public class AddChatMemberArgs
{
    public Guid ChatId { get; }
    public string AccountId { get; }
    public AccountContext OnBehalfOf { get; }
    public bool WithHistory { get; }

    public AddChatMemberArgs(Guid chatId, string accountId, AccountContext onBehalfOf, bool withHistory)
    {
        if (accountId == AccountContext.System.Id)
            throw new ArgumentException("System account cannot be added to chat", nameof(accountId));
        
        ChatId = chatId;
        AccountId = accountId ?? throw new ArgumentNullException(nameof(accountId));
        OnBehalfOf = onBehalfOf ?? throw new ArgumentNullException(nameof(onBehalfOf));
        WithHistory = withHistory;
    }
}