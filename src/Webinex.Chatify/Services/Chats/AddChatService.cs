using Webinex.Chatify.Abstractions;
using Webinex.Chatify.Common;
using Webinex.Chatify.DataAccess;
using Webinex.Chatify.Rows;

namespace Webinex.Chatify.Services.Chats;

internal interface IAddChatService
{
    Task<IReadOnlyCollection<ChatRow>> AddRangeAsync(IEnumerable<AddChatArgs> commands);
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

    public async Task<IReadOnlyCollection<ChatRow>> AddRangeAsync(IEnumerable<AddChatArgs> commands)
    {
        var rows = commands.Select(NewChat).ToArray();
        await _dbContext.Chats.AddRangeAsync(rows);

        await _eventService.FlushAsync();
        await _dbContext.SaveChangesAsync();
        return rows;
    }

    private ChatRow NewChat(AddChatArgs arg)
    {
        var members = arg.Members.Concat(new[] { arg.OnBehalfOf.Id }).Distinct().ToArray();
        return ChatRow.New(
            _eventService,
            arg.OnBehalfOf,
            arg.Name,
            members: members,
            message: arg.Message,
            requestId: arg.RequestId);
    }
}
