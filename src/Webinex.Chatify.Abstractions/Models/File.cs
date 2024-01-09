namespace Webinex.Chatify.Abstractions;

public class File : Equatable
{
    public string Name { get; protected init; } = null!;
    public int Bytes { get; protected init; }
    public string Ref { get; protected init; } = null!;

    protected File()
    {
    }

    public File(string name, int bytes, string @ref)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Bytes = bytes;
        Ref = @ref ?? throw new ArgumentNullException(nameof(@ref));
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Name;
        yield return Bytes;
        yield return Ref;
    }
}