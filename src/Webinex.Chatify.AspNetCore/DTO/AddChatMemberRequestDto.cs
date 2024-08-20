namespace Webinex.Chatify.AspNetCore;

public class AddChatMemberRequestDto
{
    public AddChatMemberRequestDto(string accountId, bool withHistory)
    {
        AccountId = accountId;
        WithHistory = withHistory;
    }

    public string AccountId { get; }
    public bool WithHistory { get; }
}