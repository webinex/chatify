using Microsoft.AspNetCore.Authorization;
using Webinex.Chatify.AspNetCore;

namespace Webinex.Chatify.Example;

[Authorize]
public class ExampleHub : ChatifyHub
{
    public ExampleHub(IChatifyHubConnections connections, IChatifyAspNetCoreContextProvider contextProviderService)
        : base(connections, contextProviderService)
    {
    }
}