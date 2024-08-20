using Webinex.Chatify.Rows;
using Webinex.Chatify.Rows.Chats;

namespace Webinex.Chatify.Services.Chats;

internal static class ChatUnreadCountUtil
{
    public static int Count(ChatActivityRow activity, ChatMemberRow[] memberships)
    {
        var count = 0;
        memberships = memberships.Where(x => !x.LastMessageIndex().HasValue || !activity.LastReadMessageIndex().HasValue ||
                                             x.LastMessageIndex() > activity.LastReadMessageIndex()).ToArray();

        foreach (var membership in memberships)
        {
            var lastMessageIndex = membership.LastMessageIndex() ?? activity.LastMessageIndex();

            if (!activity.LastReadMessageIndex().HasValue || activity.LastReadMessageIndex() < membership.FirstMessageIndex())
            {
                count += lastMessageIndex - membership.FirstMessageIndex() + 1;
                continue;
            }

            count += lastMessageIndex - activity.LastReadMessageIndex()!.Value;
        }

        return count;
    }
}
