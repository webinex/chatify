using System.Data;
using LinqToDB;
using LinqToDB.Data;
using Webinex.Chatify.Abstractions;
using Webinex.Chatify.Abstractions.Events;
using Webinex.Chatify.Common;
using Webinex.Chatify.DataAccess;
using Webinex.Chatify.Rows;
using Webinex.Chatify.Rows.Chats;

namespace Webinex.Chatify.Services.Chats;

internal interface IAddChatService
{
    Task<ChatRow> AddAsync(AddChatArgs args);
}

internal class AddChatService : IAddChatService
{
    private readonly IChatifyDataConnectionFactory _dataConnectionFactory;
    private readonly IEventService _eventService;

    public AddChatService(IEventService eventService, IChatifyDataConnectionFactory dataConnectionFactory)
    {
        _eventService = eventService;
        _dataConnectionFactory = dataConnectionFactory;
    }

    public async Task<ChatRow> AddAsync(AddChatArgs args)
    {
        await using var connection = _dataConnectionFactory.Create();
        await using var transaction = await connection.BeginTransactionAsync(IsolationLevel.ReadCommitted);

        var readForId = args.OnBehalfOf.IsSystem() ? null : args.OnBehalfOf.Id;
        var chatRow = ChatRow.New(args.Name, args.OnBehalfOf.Id);
        var messageRow = ChatMessageRow.NewChatCreated(chatRow.Id);
        var chatMeta = ChatMetaRow.New(chatRow.Id, messageRow.Id);
        var chatMembers = args.Members.Select(x => ChatMemberRow.NewInitial(chatRow, messageRow, x))
            .ToArray();
        var chatActivities = args.Members.Select(x => ChatActivityRow.NewInitial(chatRow, messageRow, x, x == readForId))
            .ToArray();
        
        await connection.InsertAsync(chatRow);
        await connection.InsertAsync(messageRow);
        await connection.InsertAsync(chatMeta);
        await connection.BulkCopyAsync(chatMembers);
        await connection.BulkCopyAsync(chatActivities);

        await transaction.CommitAsync();

        var chatValue = new NewChatMessageCreatedEvent.ChatValue(chatRow.Id, chatRow.Name, args.Members.ToArray());
        var messageBody = new MessageBody(messageRow.Text, messageRow.Files);
        _eventService.Push(new NewChatMessageCreatedEvent(messageRow.Id, chatValue, messageBody,
            messageRow.AuthorId, messageRow.SentAt, readForId));
        await _eventService.FlushAsync();
        return chatRow;
    }
}
