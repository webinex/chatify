using System.Diagnostics.CodeAnalysis;

namespace Webinex.Chatify.Rows;

internal class AccountId : Abstractions.Equatable
{
    public static readonly string SYSTEM = "chatify::system";
    
    public string Value { get; protected init; }

    public AccountId(string value)
    {
        Value = value ?? throw new ArgumentNullException(nameof(value));
    }

    public bool IsSystem() => Value == SYSTEM;

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }
    
    [return:NotNullIfNotNull("value")]
    public static implicit operator string?(AccountId? value)
    {
        return value?.Value;
    }

    [return:NotNullIfNotNull("value")]
    public static implicit operator AccountId?(string? value)
    {
        return value != null ? new AccountId(value) : null;
    }
}