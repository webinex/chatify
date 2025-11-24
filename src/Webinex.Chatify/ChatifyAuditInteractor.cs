using LinqToDB;
using Webinex.Chatify.Abstractions;
using Webinex.Chatify.Abstractions.Audit;
using Webinex.Chatify.Common;
using Webinex.Chatify.DataAccess;
using Webinex.Chatify.Rows;
using Webinex.Chatify.Rows.Chats;

namespace Webinex.Chatify;

internal class ChatifyAuditInteractor : IChatifyAuditInteractor
{
    private readonly IChatifyDataConnectionFactory _dataConnectionFactory;

    public ChatifyAuditInteractor(IChatifyDataConnectionFactory dataConnectionFactory)
    {
        _dataConnectionFactory = dataConnectionFactory;
    }

    public async Task<ListSegment<AuditChat>> ChatListSegmentAsync(
        AuditChatListSegmentQuery query)
    {
        await using var connection = _dataConnectionFactory.Create();
        var queryable = connection.ChatRows.AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.SearchString))
            queryable = queryable.Where(x => x.Name.Contains(query.SearchString));

        if (query.ContainsOneOfMembers != null)
            queryable = queryable.Where(x =>
                connection.MemberRows.Any(m => m.ChatId == x.Id && query.ContainsOneOfMembers.Contains(m.AccountId)));

        var projection = queryable.Join(connection.ChatMetaRows, chat => chat.Id, meta => meta.ChatId,
                (chat, meta) => new { chat, meta })
            .Join(connection.MessageRows, (cm) => cm.meta.LastMessageId, message => message.Id,
                (cm, message) => new { cm.chat, message })
            .Join(connection.AccountRows, x => x.message.AuthorId, account => account.Id,
                (x, author) => new { x.chat, x.message, author });

        projection = projection.OrderByDescending(x => x.message.SentAt);

        var result = await projection.ToListSegmentAsync(query.PagingRule, query.IncludeTotal);
        return result.Map(x => MapChat(x.chat, x.message, x.author));
    }

    public async Task<AuditChat?> ChatAsync(Guid id)
    {
        await using var connection = _dataConnectionFactory.Create();
        var chatRow = await connection.ChatRows.Where(x => x.Id == id).FirstOrDefaultAsync();
        return chatRow != null ? MapChat(chatRow) : null;
    }

    public async Task<ListSegment<AuditChatMessage>> ChatMessageListSegmentAsync(AuditChatMessageListSegmentQuery query)
    {
        await using var connection = _dataConnectionFactory.Create();
        var queryable = connection.MessageRows.AsQueryable()
            .Where(x => x.ChatId == query.ChatId);

        var projection = queryable.Join(connection.AccountRows, message => message.AuthorId, account => account.Id,
            (message, author) => new { message, author });

        projection = projection.OrderByDescending(x => x.message.SentAt);

        var result = await projection.ToListSegmentAsync(query.PagingRule, query.IncludeTotal);
        return result.Map(x => MapMessage(x.message, x.author));
    }

    private AuditChat MapChat(ChatRow chatRow, ChatMessageRow? lastMessageRow = null, AccountRow? accountRow = null)
    {
        return new AuditChat(chatRow.Id, chatRow.Name, chatRow.CreatedAt, chatRow.CreatedById,
            lastMessageRow != null ? MapMessage(lastMessageRow, accountRow!) : null);
    }

    private AuditChatMessage MapMessage(ChatMessageRow messageRow, AccountRow authorRow)
    {
        return new AuditChatMessage(messageRow.Id, messageRow.ChatId, messageRow.AuthorId, messageRow.SentAt,
            authorRow.ToAbstraction(), messageRow.Text, messageRow.Files);
    }
}
