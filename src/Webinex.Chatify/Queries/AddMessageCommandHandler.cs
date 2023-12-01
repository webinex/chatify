using Microsoft.EntityFrameworkCore;
using Webinex.Chatify.Abstractions;
using Webinex.Chatify.Common.Events;
using Webinex.Chatify.DataAccess;
using Webinex.Chatify.Types;
using Webinex.Coded;

namespace Webinex.Chatify.Queries;

internal class AddMessageCommandHandler
{
    private readonly ChatifyDbContext _dbContext;
    private readonly IEventService _eventService;
    private readonly AddMessageCommand[] _commands;

    public AddMessageCommandHandler(
        ChatifyDbContext dbContext,
        IEventService eventService,
        AddMessageCommand[] commands)
    {
        _dbContext = dbContext;
        _eventService = eventService;
        _commands = commands;
    }

    public async Task<Message[]> ExecuteAsync()
    {
        var chatIds = _commands.Select(x => x.ChatId);
        var members = await _dbContext.Members.Where(x => chatIds.Contains(x.ChatId)).ToArrayAsync();
        var membersByChatId = members.ToLookup(x => x.ChatId);

        var messages = _commands.Select(x =>
        {
            if (membersByChatId[x.ChatId].All(m => m.AccountId != x.OnBehalfOf.Id))
                throw CodedException.Unauthorized();

            return Message.New(
                _eventService,
                x.ChatId,
                x.OnBehalfOf.Id,
                x.Content.Text,
                x.Content.Files,
                membersByChatId[x.ChatId].Select(m => m.AccountId),
                chatCreatedEvent: null,
                requestId: x.RequestId);
        }).ToArray();

        await _dbContext.Messages.AddRangeAsync(messages);
        await _eventService.FlushAsync();
        await _dbContext.SaveChangesAsync();
        return messages;
    }
}