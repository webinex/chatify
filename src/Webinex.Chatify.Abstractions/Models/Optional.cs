using System.Diagnostics.CodeAnalysis;

namespace Webinex.Chatify.Abstractions;

public static class Optional
{
    public static Optional<TValue> NoValue<TValue>() => new(false, default);
    public static Optional<TValue> Value<TValue>(TValue? value) => new(true, value);
}

public class Optional<TValue>
{
    [MemberNotNullWhen(true, nameof(Value))]
    public bool HasValue { get; }
    public TValue? Value { get; }

    public Optional(bool hasValue, TValue? value)
    {
        HasValue = hasValue;
        Value = value;
    }
}
