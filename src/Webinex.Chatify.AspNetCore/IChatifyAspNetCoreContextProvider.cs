using Webinex.Chatify.Abstractions;

namespace Webinex.Chatify.AspNetCore;

public interface IChatifyAspNetCoreContextProvider
{
    Task<AccountContext> GetAsync();
}