namespace Webinex.Chatify.Types;

public class ChatActivityId : Equatable
{
    public Guid ChatId { get; protected init; }
    public string AccountId { get; protected init; }

    public ChatActivityId(Guid chatId, string accountId)
    {
        ChatId = chatId;
        AccountId = accountId;
    }

    public override string ToString()
    {
        return $"{ChatId}::{AccountId}";
    }

    public static ChatActivityId Parse(string value)
    {
        var split = value.Split("::");
        return new ChatActivityId(Guid.Parse(split[0]), split[1]);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return ChatId;
        yield return AccountId;
    }
}