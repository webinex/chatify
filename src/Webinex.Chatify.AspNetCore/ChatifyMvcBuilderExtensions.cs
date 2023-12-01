using Microsoft.Extensions.DependencyInjection;

namespace Webinex.Chatify.AspNetCore;

public static class ChatifyMvcBuilderExtensions
{
    public static IMvcBuilder AddChatifyAspNetCore(
        this IMvcBuilder mvcBuilder,
        Action<IChatifyAspNetCoreConfiguration> configure)
    {
        var configuration = ChatifyAspNetCoreConfiguration.GetOrCreate(mvcBuilder);
        configure(configuration);
        return mvcBuilder;
    }
}