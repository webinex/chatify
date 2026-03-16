using LinqToDB;
using System.Data;
using Webinex.Chatify.Abstractions;
using Webinex.Chatify.Abstractions.Events;
using Webinex.Chatify.Common;
using Webinex.Chatify.DataAccess;
using Webinex.Chatify.Services.Chats.Messages;

namespace Webinex.Chatify.Services.Chats.Members;

internal interface IRemoveChatMessageService
{
    Task RemoveRangeAsync(IEnumerable<RemoveChatMessageArgs> args);
}

internal class RemoveChatMessageService : IRemoveChatMessageService
{
    private readonly IEventService _eventService;
    private readonly IChatifyDataConnectionFactory _dataConnectionFactory;

    public RemoveChatMessageService(
        IEventService eventService,
        IChatifyDataConnectionFactory dataConnectionFactory)
    {
        _eventService = eventService;
        _dataConnectionFactory = dataConnectionFactory;
    }

    public async Task RemoveRangeAsync(IEnumerable<RemoveChatMessageArgs> args)
    {
        args = args.ToArray();
        foreach (var arg in args)
        {
            await RemoveAsync(arg);
        }
    }

    private async Task RemoveAsync(RemoveChatMessageArgs args)
    {
        await using var connection = _dataConnectionFactory.Create();
        await using var transaction = await connection.BeginTransactionAsync(IsolationLevel.ReadCommitted);

        var meta = await connection.GetMetaWithUpdLockAsync(args.ChatId);
        var previousMessage = await connection.GetPreviousMessageAsync(args.ChatId, args.MessageId);

        if (meta.LastMessageId == args.MessageId)
        {
            meta.Decrement(previousMessage!.Id);
            await connection.UpdateAsync(meta);
        }

        await connection.DeleteRemovedMessageReferencesAsync(args.ChatId, args.MessageId, previousMessage!.Id, previousMessage.AuthorId);

        await connection.DeleteChatMessageAsync(args.ChatId, args.MessageId);

        await transaction.CommitAsync();

        var newMessage = new ChatMessageRemovedEvent(
            args.ChatId,
            args.MessageId,
            args.OnBehalfOf.Id,
            previousMessage != null ? ChatMessageMapper.Map(previousMessage, null) : null);

        _eventService.Push(newMessage);
        await _eventService.FlushAsync();
    }
}
