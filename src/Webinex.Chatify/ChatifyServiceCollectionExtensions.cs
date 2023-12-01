using Microsoft.Extensions.DependencyInjection;

namespace Webinex.Chatify;

public static class ChatifyServiceCollectionExtensions
{
    public static IServiceCollection AddChatify(this IServiceCollection services, Action<IChatifyConfiguration> configure)
    {
        var configuration = ChatifyConfiguration.GetOrCreate(services);
        configure(configuration);
        return services;
    }
}