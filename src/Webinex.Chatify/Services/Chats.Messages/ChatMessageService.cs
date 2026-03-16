using System.Data;
using LinqToDB;
using LinqToDB.DataProvider.SqlServer;
using Webinex.Chatify.Abstractions;
using Webinex.Chatify.Abstractions.Events;
using Webinex.Chatify.Common;
using Webinex.Chatify.DataAccess;
using Webinex.Chatify.Rows.Chats;
using Webinex.Chatify.Services.Chats.Members;

namespace Webinex.Chatify.Services.Chats.Messages;

internal interface IChatMessageService
{
    Task ReadAsync(ReadChatMessageArgs chatMessageArgs);
    Task<ChatMessage[]> QueryAsync(ChatMessageQuery query);
    Task<ChatMessage[]> SendRangeAsync(IEnumerable<SendChatMessageArgs> args);
    Task<IReadOnlyDictionary<string, string>> AuthorIdByMessageIdAsync(IEnumerable<string> messageIds);
    Task RemoveRangeAsync(IEnumerable<RemoveChatMessageArgs> args);
}

internal class ChatMessageService : IChatMessageService
{
    private readonly IEventService _eventService;
    private readonly ISendMessageService _sendMessageService;
    private readonly IChatMessageQueryService _chatMessageQueryService;
    private readonly IChatifyDataConnectionFactory _dataConnectionFactory;
    private readonly IRemoveChatMessageService _removeChatMessageService;

    public ChatMessageService(
        IEventService eventService,
        ISendMessageService sendMessageService,
        IChatMessageQueryService chatMessageQueryService,
        IChatifyDataConnectionFactory dataConnectionFactory,
        IRemoveChatMessageService removeChatMessageService)
    {
        _eventService = eventService;
        _sendMessageService = sendMessageService;
        _chatMessageQueryService = chatMessageQueryService;
        _dataConnectionFactory = dataConnectionFactory;
        _removeChatMessageService = removeChatMessageService;
    }

    public async Task ReadAsync(ReadChatMessageArgs chatMessageArgs)
    {
        var messageId = ChatMessageId.Parse(chatMessageArgs.Id);

        await using var connection = _dataConnectionFactory.Create();
        await using var transaction = await connection.BeginTransactionAsync(IsolationLevel.ReadCommitted);

        var activity = await connection.ChatActivityRows.AsSqlServer().WithUpdLock()
            .FirstAsync(x => x.ChatId == messageId.ChatId && x.AccountId == chatMessageArgs.OnBehalfOf.Id);
        if (activity.LastReadMessageId != null && string.Compare(activity.LastReadMessageId, chatMessageArgs.Id,
                StringComparison.InvariantCultureIgnoreCase) >= 0)
            return;

        var previousLastReadMessageId = activity.LastReadMessageId;
        var previousLastReadMessageIndex = activity.LastReadMessageIndex();
        activity.Read(messageId);

        await connection.UpdateAsync(activity);
        await transaction.CommitAsync();

        var memberQueryable
            = connection.MemberRows.Where(x => x.ChatId == messageId.ChatId && x.AccountId == chatMessageArgs.OnBehalfOf.Id);

        if (previousLastReadMessageId != null)
            memberQueryable = memberQueryable.Where(x => x.LastMessageId == null || string.Compare(x.LastMessageId,
                previousLastReadMessageId, StringComparison.InvariantCultureIgnoreCase) >= 0);

        var members = await memberQueryable.Select(x => new { x.FirstMessageId, x.LastMessageId }).ToArrayAsync();
        var readCount = ReadCountUtil.ReadCount(
            members.Select(x => (ChatMessageId.Parse(x.FirstMessageId).Index,
                x.LastMessageId != null ? ChatMessageId.Parse(x.LastMessageId).Index : default(int?))).ToArray(),
            previousLastReadMessageIndex,
            messageId.Index);

        _eventService.Push(new ChatMessageReadEvent(messageId.ChatId, chatMessageArgs.OnBehalfOf.Id, messageId.ToString(), readCount));
        await _eventService.FlushAsync();
    }

    public async Task<ChatMessage[]> QueryAsync(ChatMessageQuery query)
    {
        return await _chatMessageQueryService.QueryAsync(query);
    }

    public async Task<ChatMessage[]> SendRangeAsync(IEnumerable<SendChatMessageArgs> args)
    {
        return await _sendMessageService.SendRangeAsync(args);
    }

    public async Task<IReadOnlyDictionary<string, string>> AuthorIdByMessageIdAsync(IEnumerable<string> messageIds)
    {
        return await _chatMessageQueryService.AuthorIdByMessageIdAsync(messageIds);
    }

    public async Task RemoveRangeAsync(IEnumerable<RemoveChatMessageArgs> args)
    {
        await _removeChatMessageService.RemoveRangeAsync(args);
    }
}
