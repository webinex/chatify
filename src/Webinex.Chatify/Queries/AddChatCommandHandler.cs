using Webinex.Chatify.Abstractions;
using Webinex.Chatify.Common.Events;
using Webinex.Chatify.DataAccess;
using Webinex.Chatify.Types;

namespace Webinex.Chatify.Queries;

internal class AddChatCommandHandler
{
    private readonly ChatifyDbContext _dbContext;
    private readonly IEventService _eventService;
    private readonly AddChatCommand[] _commands;

    public AddChatCommandHandler(ChatifyDbContext dbContext, IEventService eventService, AddChatCommand[] commands)
    {
        _dbContext = dbContext;
        _eventService = eventService;
        _commands = commands;
    }

    public async Task<IReadOnlyCollection<Chat>> ExecuteAsync()
    {
        var chats = _commands.Select(arg =>
        {
            var members = arg.Members.Concat(new[] { arg.OnBehalfOf.Id }).Distinct().ToArray();
            var chat = Chat.New(
                _eventService,
                arg.OnBehalfOf,
                arg.Name,
                members: members,
                message: arg.Message,
                requestId: arg.RequestId);
            _dbContext.Chats.Add(chat);
            return chat;
        }).ToArray();

        await _eventService.FlushAsync();
        await _dbContext.SaveChangesAsync();
        return chats;
    }
}