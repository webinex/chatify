namespace Webinex.Chatify.AspNetCore;

public class RemoveChatMemberRequestDto
{
    public RemoveChatMemberRequestDto(string accountId)
    {
        AccountId = accountId;
    }

    public string AccountId { get; }
}