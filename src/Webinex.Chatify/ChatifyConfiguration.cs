using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Webinex.Asky;
using Webinex.Chatify.Abstractions;
using Webinex.Chatify.Abstractions.Events;
using Webinex.Chatify.Common;
using Webinex.Chatify.DataAccess;
using Webinex.Chatify.Rows;
using Webinex.Chatify.Services;
using Webinex.Chatify.Services.Caches;
using Webinex.Chatify.Services.Caches.Common;
using Webinex.Chatify.Services.Chats;
using Webinex.Chatify.Services.Messages;
using Webinex.Chatify.Services.Subscribers;
using Webinex.Chatify.Services.Tasks;

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
            .AddMemoryCache()
            .AddScoped<IEventService, EventService>()
            .AddScoped<IChatify, Chatify>()
            .AddScoped<IEventSubscriber<IEnumerable<ChatCreatedEvent>>, ChatCreatedSubscriber>()
            .AddScoped<IEventSubscriber<IEnumerable<NewChatMessageCreatedEvent>>, NewChatMessageCreatedSubscriber>()
            .AddScoped<IEventSubscriber<IEnumerable<MessageSentEvent>>, MessageSentSubscriber>()
            .AddSingleton<IAskyFieldMap<ChatActivityRow>, ChatActivityRowFieldMap>()
            .AddSingleton<IAskyFieldMap<DeliveryRow>, DeliveryRowFieldMap>()
            .AddSingleton<IAskyFieldMap<ChatRow>, ChatRowFieldMap>();

        services
            .AddScoped<IAuthorizationPolicy, AuthorizationPolicy>()
            .AddScoped<IAccountService, AccountService>()
            .AddScoped<IMessageService, MessageService>()
            .AddScoped<ISendMessageService, SendMessageService>()
            .AddScoped<IMessageQueryService, MessageQueryService>()
            .AddScoped<IChatService, ChatService>()
            .AddScoped<IChatQueryService, ChatQueryService>()
            .AddScoped<IAddChatService, AddChatService>()
            .AddScoped<IMemberService, MemberService>();


        services
            .AddSingleton<ChatifyQueue>()
            .AddHostedService(x => x.GetRequiredService<ChatifyQueue>())
            .AddSingleton<IChatifyQueue>(x => x.GetRequiredService<ChatifyQueue>())
            .AddScoped<IJob<AddMemberTask>, AddMemberJob>()
            .AddScoped<IJob<RemoveMemberTask>, RemoveMemberJob>();

        services
            .AddSingleton<IEntityCache<AccountRow>, EntityMemoryCache<AccountRow>>()
            .AddSingleton(new EntityMemoryCacheSettings<AccountRow>(x => x.Id, TimeSpan.FromMinutes(15), "account"))
            .AddSingleton<IEntityCache<ChatMembersCacheEntry>, EntityMemoryCache<ChatMembersCacheEntry>>()
            .AddSingleton(new EntityMemoryCacheSettings<ChatMembersCacheEntry>(x => x.ChatId.ToString(),
                TimeSpan.FromMinutes(15), "chat::members"));
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
