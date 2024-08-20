namespace Webinex.Chatify.Services.Chats.Messages;

internal static class ReadCountUtil
{
    public static int ReadCount(
        IEnumerable<(int firstMessageIndex, int? lastMessageIndex)> members,
        int? previousLastReadMessageIndex,
        int newLastReadMessageIndex)
    {
        if (previousLastReadMessageIndex == newLastReadMessageIndex)
            return 0;

        var result = 0;

        foreach (var (firstMessageIndex, lastMessageIndex) in members)
        {
            if (lastMessageIndex <= previousLastReadMessageIndex)
                continue;

            if (firstMessageIndex > newLastReadMessageIndex)
                continue;
            
            if (firstMessageIndex == lastMessageIndex)
            {
                result++;
                continue;
            }
            
            var firstReadIndex = firstMessageIndex < previousLastReadMessageIndex
                ? previousLastReadMessageIndex.Value + 1
                : firstMessageIndex;
            
            var lastReadIndex = lastMessageIndex < newLastReadMessageIndex
                ? lastMessageIndex.Value
                : newLastReadMessageIndex;
            
            result += lastReadIndex - firstReadIndex + 1;
        }

        return result;
    }
}
