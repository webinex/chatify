using Webinex.Chatify.Services.Chats.Messages;

namespace Webinex.Chatify.Tests;

public class ReadCountUtilTests
{
    [TestCaseSource(nameof(TestCases))]
    public void Test(TestCase testCase)
    {
        var result = ReadCountUtil.ReadCount(
            testCase.Members.Select(x => (x.FirstMessageIndex, x.LastMessageIndex)).ToArray(),
            testCase.PreviousLastReadMessageIndex, testCase.NewLastReadMessageIndex);

        Assert.That(result, Is.EqualTo(testCase.ExpectedReadCount));
    }

    public static IEnumerable<TestCase> TestCases()
    {
        yield return new TestCase(
            [
                new Member(1, 1),
                new Member(2, 2),
                new Member(3, 3)
            ],
            1,
            3,
            2);
        
        yield return new TestCase(
            [
                new Member(1, 5),
                new Member(8, 10),
                new Member(11, null)
            ],
            3,
            12,
            7);
        
        yield return new TestCase(
            [
                new Member(1, 1),
            ],
            1,
            1,
            0);
    }

    public record TestCase(
        Member[] Members,
        int? PreviousLastReadMessageIndex,
        int NewLastReadMessageIndex,
        int ExpectedReadCount);

    public record Member(
        int FirstMessageIndex,
        int? LastMessageIndex);
}
