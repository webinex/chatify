using Microsoft.AspNetCore.Authorization;
using Webinex.Chatify.AspNetCore;
using Webinex.Chatify.AspNetCore.SignalR;

namespace Webinex.Chatify.Example;

[Authorize]
public class ExampleHub : ChatifyHub
{
    public ExampleHub(IChatifyHubConnections connections, IChatifyAspNetCoreContext contextService)
        : base(connections, contextService)
    {
    }
}