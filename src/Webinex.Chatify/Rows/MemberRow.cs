using Webinex.Chatify.Abstractions;
using Webinex.Chatify.Common;

namespace Webinex.Chatify.Rows;

internal class MemberRow
{
    private MemberAbstraction _abstraction;

    public Guid ChatId { get; protected set; }
    public string AccountId { get; protected init; } = null!;
    public string AddedById { get; protected init; } = null!;
    public DateTimeOffset AddedAt { get; protected init; }

    protected MemberRow()
    {
        _abstraction = new MemberAbstraction(this);
    }

    private MemberRow(Guid chatId, string accountId, string addedById, DateTimeOffset addedAt)
        : this()
    {
        ChatId = chatId;
        AccountId = accountId ?? throw new ArgumentNullException(nameof(accountId));
        AddedById = addedById ?? throw new ArgumentNullException(nameof(addedById));
        AddedAt = addedAt;
    }

    // internal static MemberRow NewAdded(IEventService eventService, Guid chatId, string accountId, string addedById)
    // {
    //     if (accountId == AccountContext.System.Id)
    //         throw new ArgumentException("Cannot add system account to chat", nameof(accountId));
    //
    //     var member = new MemberRow(chatId, accountId, addedById, DateTimeOffset.UtcNow);
    //     eventService.Push(new MemberAddedEvent(member.ChatId, member.AccountId));
    //     return member;
    // }

    internal static MemberRow NewInitial(
        IEventService eventService,
        Guid chatId,
        string accountId,
        string addedById,
        DateTimeOffset addedAt)
    {
        if (accountId == AccountContext.System.Id)
            throw new ArgumentException("Cannot add system account to chat", nameof(accountId));

        var member = new MemberRow(chatId, accountId, addedById, addedAt);
        return member;
    }

    public static MemberRow NewByJob(Guid chatId, string accountId, string addedById, DateTimeOffset addedAt)
    {
        return new MemberRow(chatId, accountId, addedById, addedAt);
    }

    public Member ToAbstraction()
    {
        return _abstraction;
    }

    private class MemberAbstraction : Member
    {
        private readonly MemberRow _row;

        public override Guid ChatId => _row.ChatId;
        public override string AccountId => _row.AccountId;
        public override string AddedById => _row.AddedById;
        public override DateTimeOffset AddedAt => _row.AddedAt;

        public MemberAbstraction(MemberRow row)
        {
            _row = row;
        }
    }
}
