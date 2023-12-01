using Webinex.Chatify.Common.Events;
using Webinex.Chatify.Types.Events;

namespace Webinex.Chatify.Types;

public class Member
{
    public Guid ChatId { get; protected set; }
    public string AccountId { get; protected init; } = null!;
    public string AddedById { get; protected init; } = null!;
    public DateTimeOffset AddedAt { get; protected init; }

    protected Member()
    {
    }

    private Member(Guid chatId, string accountId, string addedById, DateTimeOffset addedAt)
    {
        ChatId = chatId;
        AccountId = accountId ?? throw new ArgumentNullException(nameof(accountId));
        AddedById = addedById ?? throw new ArgumentNullException(nameof(addedById));
        AddedAt = addedAt;
    }

    public static Member NewAdded(IEventService eventService, Guid chatId, string accountId, string addedById)
    {
        var member = new Member(chatId, accountId, addedById, DateTimeOffset.UtcNow);
        eventService.Push(new MemberAddedEvent(member.ChatId, member.AccountId));
        return member;
    }

    public static Member New(Guid chatId, string accountId, string addedById, DateTimeOffset addedAt)
    {
        var member = new Member(chatId, accountId, addedById, addedAt);
        return member;
    }
}