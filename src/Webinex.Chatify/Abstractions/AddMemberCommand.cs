namespace Webinex.Chatify.Abstractions;

public class AddMemberCommand
{
    public Guid ChatId { get; }
    public string AccountId { get; }
    public AccountContext OnBehalfOf { get; }
    public bool WithHistory { get; }

    public AddMemberCommand(Guid chatId, string accountId, AccountContext onBehalfOf, bool withHistory)
    {
        ChatId = chatId;
        AccountId = accountId;
        OnBehalfOf = onBehalfOf;
        WithHistory = withHistory;
    }
}