using Microsoft.AspNetCore.SignalR;

namespace Webinex.Chatify.AspNetCore;

public abstract class ChatifyHub : Hub
{
    private readonly IChatifyHubConnections _connections;
    private readonly IChatifyAspNetCoreContextProvider _contextProviderService;

    protected ChatifyHub(IChatifyHubConnections connections, IChatifyAspNetCoreContextProvider contextProviderService)
    {
        _connections = connections;
        _contextProviderService = contextProviderService;
    }

    public override async Task OnConnectedAsync()
    {
        var context = await _contextProviderService.GetAsync();
        _connections.Add(context.Id, Context.ConnectionId);
        await base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        _connections.Remove(Context.UserIdentifier!, Context.ConnectionId);
        return base.OnDisconnectedAsync(exception);
    }

    [HubMethodName("ConnectToThread")]
    public async Task ConnectToThreadAsync(string id)
        => await Groups.AddToGroupAsync(Context.ConnectionId, HubGroupNames.Thread(id));

    [HubMethodName("DisconnectFromThread")]
    public async Task DisconnectFromThreadAsync(string id)
        => await Groups.RemoveFromGroupAsync(Context.ConnectionId, HubGroupNames.Thread(id));
}
