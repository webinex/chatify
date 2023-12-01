namespace Webinex.Chatify.AspNetCore.Controller;

public class RemoveChatMemberRequestDto
{
    public RemoveChatMemberRequestDto(string accountId, bool deleteHistory)
    {
        AccountId = accountId;
        DeleteHistory = deleteHistory;
    }

    public string AccountId { get; }
    public bool DeleteHistory { get; }
}