namespace Webinex.Chatify.Abstractions;

public class AutoReply : Equatable
{
    public Period<DateTimeOffset> Period { get; private set; } = null!;
    public string Text { get; private set; } = null!;

    private AutoReply() { }

    public AutoReply(
        Period<DateTimeOffset> period,
        string text)
    {
        Period = period;
        Text = text;
    }

    public AutoReply Clone() => new(Period, Text);

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Period;
        yield return Text;
    }
}