namespace Webinex.Chatify.Abstractions;

public class Period<T> : Equatable
{
    public T Start { get; }
    public T End { get; }

    public Period(T start, T end)
    {
        Validate(start, end);

        Start = start;
        End = end;
    }

    private static void Validate(T start, T end)
    {
        if (typeof(T).IsAssignableTo(typeof(IComparable)))
        {
            var result = Comparer<T>.Default.Compare(start, end);
            if (result > 0)
                throw new ArgumentException($"Start ({start}) must be <= End ({end}).");
        }   
    }

    public bool Contains(T value)
    {
        if (typeof(T).IsAssignableTo(typeof(IComparable)))
        {
            return Comparer<T>.Default.Compare(value, Start) >= 0
                && Comparer<T>.Default.Compare(value, End) <= 0;
        }

        throw new InvalidOperationException(
            $"Type {typeof(T)} must implement IComparable or IComparable<T> to use Contains().");
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Start;
        yield return End;
    }
}
