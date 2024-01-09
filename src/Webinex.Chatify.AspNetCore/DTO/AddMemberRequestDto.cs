namespace Webinex.Chatify.AspNetCore;

public class AddMemberRequestDto
{
    public AddMemberRequestDto(string accountId)
    {
        AccountId = accountId;
    }

    public string AccountId { get; }
}