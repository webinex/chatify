using LinqToDB;
using Microsoft.Extensions.DependencyInjection;
using Webinex.Asky;
using Webinex.Chatify.Abstractions;
using Webinex.Chatify.Abstractions.Events;
using Webinex.Chatify.Common;
using Webinex.Chatify.DataAccess;
using Webinex.Chatify.Rows;
using Webinex.Chatify.Rows.Chats;
using Webinex.Chatify.Services;
using Webinex.Chatify.Services.Chats;
using Webinex.Chatify.Services.Chats.Caches;
using Webinex.Chatify.Services.Chats.Members;
using Webinex.Chatify.Services.Chats.Messages;
using Webinex.Chatify.Services.Common.Caches;
using Webinex.Chatify.Services.Threads;

namespace Webinex.Chatify;

public interface IChatifyConfiguration
{
    IServiceCollection Services { get; }
    IChatifyConfiguration UseDataConnection(Func<DataOptions, DataOptions> configure);
    IChatifyConfiguration AddAudit();
}

internal class ChatifyConfiguration : IChatifyConfiguration
{
    private ChatifyConfiguration(IServiceCollection services)
    {
        Services = services;

        services
            .AddMemoryCache()
            .AddSingleton<IChatifyDataConnectionFactory, ChatifyDataConnectionFactory>()
            .AddScoped<IEventService, EventService>()
            .AddScoped<IChatify, Chatify>()
            .AddSingleton<IAskyFieldMap<ChatActivityRow>, ChatActivityRowFieldMap>()
            .AddSingleton<IAskyFieldMap<ChatRow>, ChatRowFieldMap>()
            .AddSingleton<IAskyFieldMap<ChatMessageRow>, ChatMessageRowFieldMap>();

        services
            .AddScoped<IAuthorizationPolicy, AuthorizationPolicy>()
            .AddScoped<IAccountService, AccountService>()
            .AddScoped<IChatMessageService, ChatMessageService>()
            .AddScoped<ISendMessageService, SendMessageService>()
            .AddScoped<IChatMessageQueryService, ChatMessageQueryService>()
            .AddScoped<IChatService, ChatService>()
            .AddScoped<IChatQueryService, ChatQueryService>()
            .AddScoped<IAddChatService, AddChatService>()
            .AddScoped<IChatMemberService, ChatMemberService>()
            .AddScoped<IGetChatMemberService, GetChatMemberService>()
            .AddScoped<IAddChatMemberService, AddChatMemberService>()
            .AddScoped<IRemoveChatMemberService, RemoveChatMemberService>();

        services
            .AddSingleton<IEntityCache<AccountRow>, EntityMemoryCache<AccountRow>>()
            .AddSingleton(new EntityMemoryCacheSettings<AccountRow>(x => x.Id, TimeSpan.FromMinutes(15), "account"))
            .AddSingleton<IEntityCache<ChatMembersCacheEntry>, EntityMemoryCache<ChatMembersCacheEntry>>()
            .AddSingleton(new EntityMemoryCacheSettings<ChatMembersCacheEntry>(x => x.ChatId.ToString(),
                TimeSpan.FromMinutes(15), "chat::members"));

        services
            .AddScoped<IThreadService, ThreadService>()
            .AddSingleton<IAskyFieldMap<ThreadQueryView>, ThreadQueryViewFieldMap>();

        services.AddScoped<IEventSubscriber<IEnumerable<ChatMessageSentEvent>>, AutoReplyEventSubscriber>();
    }

    public IServiceCollection Services { get; }

    public IChatifyConfiguration UseDataConnection(Func<DataOptions, DataOptions> configure)
    {
        var options = new DataOptions()
            .UseMappingSchema(ChatifyDataConnection.Schema());
        options = configure(options);
        var chatifyOptions = new DataOptions<ChatifyDataConnection>(options);
        Services.AddSingleton(chatifyOptions);
        return this;
    }

    public IChatifyConfiguration AddAudit()
    {
        Services.AddScoped<IChatifyAuditInteractor, ChatifyAuditInteractor>();
        return this;
    }

    internal static ChatifyConfiguration GetOrCreate(IServiceCollection services)
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
