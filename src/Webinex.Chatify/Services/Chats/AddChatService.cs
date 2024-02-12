using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Webinex.Chatify.Abstractions;
using Webinex.Chatify.Abstractions.Events;
using Webinex.Chatify.Common;
using Webinex.Chatify.DataAccess;
using Webinex.Chatify.Rows;
using File = Webinex.Chatify.Abstractions.File;

namespace Webinex.Chatify.Services.Chats;

internal interface IAddChatService
{
    Task<ChatRow> AddAsync(AddChatArgs args);
}

internal class AddChatService : IAddChatService
{
    private readonly ChatifyDbContext _dbContext;
    private readonly IEventService _eventService;

    public AddChatService(ChatifyDbContext dbContext, IEventService eventService)
    {
        _dbContext = dbContext;
        _eventService = eventService;
    }

    public async Task<ChatRow> AddAsync(AddChatArgs args)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted);
        var chatId = Guid.NewGuid();
        var messageId = MessageId.New(chatId, 0);
        var now = messageId.CreatedAt;
        var messageText = "chatify://chat-created";

        var parameters = new List<SqlParameter>
        {
            new("chatId", chatId),
            new("chatName", args.Name),
            new("now", now),
            new("userId", args.OnBehalfOf.Id),
            new("messageId", messageId.ToString()),
            new("messageText", messageText),
            new("systemId", AccountContext.System.Id),
        };
        
        foreach (var (member, index) in args.Members.Select((x, index) => (x, index)))
        {
            parameters.Add(new($"member_{index}_id", member));
        }

        var membersCount = args.Members.Count;

        string NewChatActivityValue(int index)
        {
            return $"(@chatId, @member_{index}_id, @systemId, @messageId, 0, NULL, NULL, 1)";
        }
        
        var chatActivityValues = string.Join(", ", Enumerable.Range(0, membersCount).Select(NewChatActivityValue));

#pragma warning disable EF1002
        await _dbContext.Database.ExecuteSqlRawAsync(
#pragma warning restore EF1002
            $"""
             insert into chatify.Chats(Id, Name, CreatedAt, CreatedById)
             values (@chatId, @chatName, @now, @userId)

             insert into chatify.ChatMeta(ChatId, LastIndex)
             values (@chatId, 0)

             insert into chatify.Messages(Id, ChatId, Text, AuthorId, SentAt, [Index], Files)
             values (@messageId, @chatId, @messageText, @systemId, @now, 0, '[]')

             insert into chatify.Members(Id, ChatId, AccountId, AddedById, AddedAt, FirstMessageId, FirstMessageIndex, LastMessageId, LastMessageIndex)
             values {string.Join(", ", Enumerable.Range(0, membersCount).Select(index => $"(NEWID(), @chatId, @member_{index}_id, @userId, @now, @messageId, 0, NULL, NULL)"))}

             insert into chatify.ChatActivities(
                                                ChatId,
                                                AccountId,
                                                LastMessageFromId,
                                                LastMessageId,
                                                LastMessageIndex,
                                                LastReadMessageId,
                                                LastReadMessageIndex,
                                                Active)
             values {chatActivityValues}
             """,
            parameters);

        await transaction.CommitAsync();

        var chatValue = new NewChatMessageCreatedEvent.ChatValue(chatId, args.Name, args.Members.ToArray());
        var messageBody = new MessageBody(messageText, Array.Empty<File>());
        _eventService.Push(new NewChatMessageCreatedEvent(messageId.ToString(), chatValue, messageBody,
            AccountContext.System.Id, now, args.RequestId));
        await _eventService.FlushAsync();
        return new ChatRow(chatId, args.Name, now, args.OnBehalfOf.Id);
    }
}
