using System.Data;
using LinqToDB;
using Webinex.Chatify.Abstractions;
using Webinex.Chatify.Abstractions.Events;
using Webinex.Chatify.Common;
using Webinex.Chatify.DataAccess;
using Webinex.Chatify.Rows.Chats;

namespace Webinex.Chatify.Services.Chats.Messages;

internal interface ISendMessageService
{
    Task<ChatMessage[]> SendRangeAsync(IEnumerable<SendChatMessageArgs> argEnumerable, bool isAutoReply = false);
}

internal class SendMessageService : ISendMessageService
{
    private readonly IEventService _eventService;
    private readonly IChatifyDataConnectionFactory _dataConnectionFactory;

    public SendMessageService(
        IEventService eventService,
        IChatifyDataConnectionFactory dataConnectionFactory)
    {
        _eventService = eventService;
        _dataConnectionFactory = dataConnectionFactory;
    }

    public async Task<ChatMessage[]> SendRangeAsync(IEnumerable<SendChatMessageArgs> argEnumerable, bool isAutoReply = false)
    {
        var args = argEnumerable.ToArray();
        var result = new LinkedList<ChatMessage>();
        foreach (var arg in args)
        {
            result.AddLast(await SendAsync(arg, isAutoReply));
        }

        return result.ToArray();
    }

    private async Task<ChatMessage> SendAsync(SendChatMessageArgs args, bool isAutoReply = false)
    {
        await using var connection = _dataConnectionFactory.Create();
        await using var transaction = await connection.BeginTransactionAsync(IsolationLevel.ReadCommitted);

        var meta = await connection.GetMetaWithUpdLockAsync(args.ChatId);
        var messageRow = ChatMessageRow.NewBody(args.ChatId, meta.LastIndex + 1, args.OnBehalfOf.Id, args.Body.Text, args.Body.Files);
        meta.Increment(messageRow.Id);
        await connection.SendMessageAsync(messageRow, readForId: args.OnBehalfOf.Id);
        await connection.UpdateAsync(meta);

        await transaction.CommitAsync();
        
        _eventService.Push(new ChatMessageSentEvent(messageRow.Id, messageRow.ChatId, new MessageBody(messageRow.Text, messageRow.Files), messageRow.AuthorId,
            messageRow.SentAt, isAutoReply));
        await _eventService.FlushAsync();
        return ChatMessageMapper.Map(messageRow, null);
    }
}
