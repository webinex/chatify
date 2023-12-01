using Microsoft.AspNetCore.SignalR;

namespace Webinex.Chatify.AspNetCore.SignalR;

public abstract class ChatifyHub : Hub
{
    private readonly IChatifyHubConnections _connections;
    private readonly IChatifyAspNetCoreContext _contextService;

    protected ChatifyHub(IChatifyHubConnections connections, IChatifyAspNetCoreContext contextService)
    {
        _connections = connections;
        _contextService = contextService;
    }
    
    public override async Task OnConnectedAsync()
    {
        var context = await _contextService.GetAsync();
        _connections.Add(context.Id, Context.ConnectionId);
        await base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        _connections.Remove(Context.UserIdentifier!, Context.ConnectionId);
        return base.OnDisconnectedAsync(exception);
    }
}