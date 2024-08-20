namespace Webinex.Chatify.DataAccess;

internal static class TaskExtensions
{
    public static Task<int> AssertResult(this Task<int> task, int expectedValue)
    {
        return task.ContinueWith(t =>
        {
            if (t.Result != expectedValue)
            {
                throw new InvalidOperationException($"Expected {expectedValue} but got {t.Result}");
            }

            return t.Result;
        });
    }
}
