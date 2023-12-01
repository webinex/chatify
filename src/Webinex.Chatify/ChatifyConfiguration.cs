using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Webinex.Asky;
using Webinex.Chatify.Abstractions;
using Webinex.Chatify.Common.Events;
using Webinex.Chatify.DataAccess;
using Webinex.Chatify.Queries;
using Webinex.Chatify.Tasks;
using Webinex.Chatify.Types.Events;
using Webinex.Chatify.Types.Subscribers;

namespace Webinex.Chatify;

public interface IChatifyConfiguration
{
    IServiceCollection Services { get; }

    IChatifyConfiguration UseDbContext(Action<DbContextOptionsBuilder> configure);
}

internal class ChatifyConfiguration : IChatifyConfiguration
{
    private ChatifyConfiguration(IServiceCollection services)
    {
        Services = services;

        services
            .AddScoped<IEventService, EventService>()
            .AddScoped<IChatify, Chatify>()
            .AddScoped<ISubscriber<IEnumerable<ChatCreatedEvent>>, ChatCreatedSubscriber>()
            .AddScoped<ISubscriber<IEnumerable<MessageCreatedEvent>>, MessageCreatedSubscriber>()
            .AddScoped<IAskyFieldMap<ChatQueryHandler.View>, ChatQueryHandler.FieldMap>()
            .AddScoped<IAskyFieldMap<MessageQueryHandler.View>, MessageQueryHandler.FieldMap>()
            .AddSingleton<ChatifyQueue>()
            .AddHostedService(x => x.GetRequiredService<ChatifyQueue>())
            .AddSingleton<IChatifyQueue>(x => x.GetRequiredService<ChatifyQueue>());
    }

    public IServiceCollection Services { get; }

    public IChatifyConfiguration UseDbContext(Action<DbContextOptionsBuilder> configure)
    {
        Services.AddDbContext<ChatifyDbContext>(configure);
        return this;
    }

    public static ChatifyConfiguration GetOrCreate(IServiceCollection services)
    {
        services = services ?? throw new ArgumentNullException(nameof(services));

        var instance = (ChatifyConfiguration?)services.FirstOrDefault(x =>
                x.ImplementationInstance?.GetType() == typeof(ChatifyConfiguration))
            ?.ImplementationInstance;

        if (instance != null)
            return instance;

        instance = new ChatifyConfiguration(services);
        services.AddSingleton(instance);
        return instance;
    }
}