using Webinex.Chatify.Abstractions;

namespace Webinex.Chatify.Rows;

internal class MessageId : Equatable
{
    public DateTimeOffset SentAt { get; protected init; }
    public string Uniq { get; protected init; } = null!;

    public MessageId(DateTimeOffset sentAt, string uniq)
    {
        SentAt = sentAt.ToUniversalTime();
        Uniq = uniq;
    }

    public MessageId(DateTimeOffset sentAt)
    {
        SentAt = sentAt.ToUniversalTime();
        Uniq = Guid.NewGuid().ToString();
    }
    
    public static MessageId New()
    {
        return new MessageId(DateTimeOffset.UtcNow);
    }

    protected MessageId()
    {
    }

    public override string ToString()
    {
        return SentAt.ToString("s") + "::" + Uniq;
    }

    public static MessageId Parse(string value)
    {
        var split = value.Split("::");
        return new MessageId(DateTimeOffset.Parse(split[0]), split[1]);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return SentAt;
        yield return Uniq;
    }
}