using System.Linq.Expressions;
using Webinex.Chatify.Abstractions;
using Webinex.Chatify.Common;

namespace Webinex.Chatify.Rows;

internal class ChatActivityRowId : Equatable
{
    public ChatActivityRowId(Guid chatId, string accountId)
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

internal class ChatActivityRow
{
    public Guid ChatId { get; protected init; }
    public string AccountId { get; protected init; } = null!;
    public string LastMessageFromId { get; protected set; } = null!;
    public string LastMessageId { get; protected set; } = null!;
    public int LastMessageIndex { get; protected set; }
    public string? LastReadMessageId { get; protected set; }
    public int? LastReadMessageIndex { get; protected set; }
    public bool Active { get; protected set; }
    public MessageRow LastMessage { get; protected set; } = null!;
    public ChatRow? Chat { get; protected set; } = null!;

    protected ChatActivityRow()
    {
    }

    public ChatActivityRow(Guid chatId, string accountId, string lastMessageFromId, string lastMessageId, bool active, string? lastReadMessageId)
    {
        ChatId = chatId;
        AccountId = accountId;
        LastMessageFromId = lastMessageFromId;
        LastMessageId = lastMessageId;
        LastReadMessageId = lastReadMessageId;
        Active = active;
    }

    internal static Expression<Func<ChatActivityRow, bool>> ById(IEnumerable<ChatActivityRowId> ids)
    {
        ids = ids.Distinct().ToArray();
        if (!ids.Any()) throw new ArgumentException("Might be at least one", nameof(ids));
        var expressions = ids.Select(x => ById(x.ChatId, x.AccountId)).ToArray();
        return expressions.Length > 1 ? BoolExpressions.And(expressions) : expressions[0];
    }

    internal static Expression<Func<ChatActivityRow, bool>> ById(Guid chatId, string accountId)
    {
        return x => x.ChatId == chatId && x.AccountId == accountId;
    }
}
