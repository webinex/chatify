using Webinex.Chatify.Rows;
using Webinex.Chatify.Rows.Chats;
using Webinex.Chatify.Services.Chats;

namespace Webinex.Chatify.Tests;

internal class ChatUnreadCountUtilTests
{
    private static readonly Guid Id = Guid.NewGuid();

    [TestCaseSource(nameof(TestCases))]
    public void Test(TestCase testCase)
    {
        var result = ChatUnreadCountUtil.Count(testCase.Activity, testCase.Members);
        Assert.That(result, Is.EqualTo(testCase.ExpectedResult), testCase.ToString());
    }

    public static IEnumerable<TestCase> TestCases()
    {
        yield return new TestCase(
            [NewMember(0, 3), NewMember(5, null)],
            NewActivity(6, null),
            6);

        yield return new TestCase(
            [NewMember(3, 3), NewMember(5, null)],
            NewActivity(6, null),
            3);

        yield return new TestCase(
            [NewMember(0, 2)],
            NewActivity(2, null),
            3);

        yield return new TestCase(
            [NewMember(0, null)],
            NewActivity(2, null),
            3);

        yield return new TestCase(
            [NewMember(0, 3), NewMember(5, null)],
            NewActivity(6, 2),
            3);

        yield return new TestCase(
            [NewMember(0, 3), NewMember(5, null)],
            NewActivity(6, 3),
            2);

        yield return new TestCase(
            [NewMember(0, 1), NewMember(3, 3), NewMember(5, null)],
            NewActivity(6, 5),
            1);

        yield return new TestCase(
            [NewMember(0, 3), NewMember(5, 6)],
            NewActivity(6, 2),
            3);
    }

    internal record TestCase(
        ChatMemberRow[] Members,
        ChatActivityRow Activity,
        int ExpectedResult)
    {
        public override string ToString()
        {
            return "Test Case:\n\n" +
                   "  Members:\n" +
                   $"{string.Join("\n", Members.Select((x, index) => $"  {index + 1}. FirstMessageIndex = {x.FirstMessageIndex}, LastMessageIndex = {x.LastMessageIndex}"))}\n\n" +
                   "  Activity:\n" +
                   $"  LastMessageIndex: {Activity.LastMessageIndex}\n" +
                   $"  LastReadMessageIndex: {Activity.LastReadMessageIndex}\n\n" +
                   $"  ExpectedResult: {ExpectedResult}\n";
        }
    };

    private static ChatActivityRow NewActivity(int lastMessageIndex, int? lastReadMessageIndex) => new(
        Id,
        Id.ToString(),
        Id.ToString(),
        ChatMessageId.New(Id, lastMessageIndex).ToString(),
        true,
        lastReadMessageIndex.HasValue
            ? ChatMessageId.New(Id, lastReadMessageIndex.Value).ToString()
            : null);

    private static ChatMemberRow NewMember(int firstMessageIndex, int? lastMessageIndex) => new(
        Guid.NewGuid(),
        Id,
        Id.ToString(),
        Id.ToString(),
        DateTimeOffset.UtcNow,
        ChatMessageId.New(Id, firstMessageIndex).ToString(),
        lastMessageIndex.HasValue
            ? ChatMessageId.New(Id, lastMessageIndex.Value).ToString()
            : null);
}
