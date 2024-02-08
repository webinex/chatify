namespace Webinex.Chatify.AspNetCore;

public class AddMemberRequestDto
{
    public AddMemberRequestDto(string accountId, bool withHistory)
    {
        AccountId = accountId;
        WithHistory = withHistory;
    }

    public string AccountId { get; }
    public bool WithHistory { get; }
}