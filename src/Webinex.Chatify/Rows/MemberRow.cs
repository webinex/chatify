using System.Linq.Expressions;
using Webinex.Chatify.Abstractions;
using Webinex.Chatify.Common;

namespace Webinex.Chatify.Rows;

internal class MemberId : Equatable
{
    public MemberId(Guid chatId, string accountId)
    {
        ChatId = chatId;
        AccountId = accountId;
    }

    public Guid ChatId { get; }
    public string AccountId { get; }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return ChatId;
        yield return AccountId;
    }
}

internal class MemberRow
{
    private MemberAbstraction _abstraction;

    public Guid Id { get; protected init; }
    public Guid ChatId { get; protected set; }
    public string AccountId { get; protected init; } = null!;
    public string AddedById { get; protected init; } = null!;
    public DateTimeOffset AddedAt { get; protected init; }
    public string FirstMessageId { get; protected init; } = null!;
    public int FirstMessageIndex { get; protected init; }
    public string? LastMessageId { get; protected set; }
    public int? LastMessageIndex { get; protected set; }

    protected MemberRow()
    {
        _abstraction = new MemberAbstraction(this);
    }

    private MemberRow(
        Guid id,
        Guid chatId,
        string accountId,
        string addedById,
        DateTimeOffset addedAt,
        string firstMessageId,
        string? lastMessageId)
        : this()
    {
        Id = id;
        ChatId = chatId;
        AccountId = accountId ?? throw new ArgumentNullException(nameof(accountId));
        AddedById = addedById ?? throw new ArgumentNullException(nameof(addedById));
        AddedAt = addedAt;
        FirstMessageId = firstMessageId ?? throw new ArgumentNullException(nameof(firstMessageId));
        LastMessageId = lastMessageId;
    }

    public static string FormatSqlInsert(
        (string ChatIdRef, string AccountIdRef, string AddedByIdRef, string AddedAtRef, string FirstMessageIdRef, string FirstMessageIndexValue) args)
    {
        return $"""
                insert into chatify.Members(Id, ChatId, AccountId, AddedById, AddedAt, FirstMessageId, FirstMessageIndex, LastMessageId, LastMessageIndex)
                values (NEWID(), {args.ChatIdRef}, {args.AccountIdRef}, {args.AddedByIdRef}, {args.AddedAtRef}, {args.FirstMessageIdRef}, {args.FirstMessageIndexValue}, NULL, NULL)
                """;
    }

    public static string FormatSqlDelete((string ChatIdRef, string AccountIdRef) args)
    {
        return $"delete from chatify.Members where ChatId = {args.ChatIdRef} and AccountId = {args.AccountIdRef}";
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
        DateTimeOffset addedAt,
        string firstMessageId)
    {
        if (accountId == AccountContext.System.Id)
            throw new ArgumentException("Cannot add system account to chat", nameof(accountId));

        var member = new MemberRow(Guid.NewGuid(), chatId, accountId, addedById, addedAt, firstMessageId, null);
        return member;
    }

    public static MemberRow NewAdded(
        Guid chatId,
        string accountId,
        string addedById,
        DateTimeOffset addedAt,
        string firstMessageId)
    {
        return new MemberRow(Guid.NewGuid(), chatId, accountId, addedById, addedAt, firstMessageId, null);
    }

    public void Exclude(string lastMessageId)
    {
        LastMessageId = lastMessageId;
    }

    public Member ToAbstraction()
    {
        return _abstraction;
    }

    internal static Expression<Func<MemberRow, bool>> ByMember(IEnumerable<MemberId> ids)
    {
        ids = ids.Distinct().ToArray();
        if (!ids.Any()) throw new ArgumentException("Might be at least one", nameof(ids));

        var expressions = ids.Select(id => ByMember(id.ChatId, id.AccountId)).ToArray();
        return expressions.Length > 1 ? BoolExpressions.And(expressions) : expressions[0];
    }

    internal static Expression<Func<MemberRow, bool>> ByMember(Guid chatId, string accountId)
    {
        return row => row.AccountId == accountId && row.ChatId == chatId;
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
