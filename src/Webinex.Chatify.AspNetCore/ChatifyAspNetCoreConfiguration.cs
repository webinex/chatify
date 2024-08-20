using System.Reflection;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Webinex.Chatify.Abstractions.Events;

namespace Webinex.Chatify.AspNetCore;

public interface IChatifyAspNetCoreConfiguration
{
    IMvcBuilder MvcBuilder { get; }

    IChatifyAspNetCoreConfiguration AddController();

    IChatifyAspNetCoreConfiguration AddSignalR<THub>()
        where THub : ChatifyHub;
}

internal class ChatifyAspNetCoreConfiguration : IChatifyAspNetCoreConfiguration
{
    private ChatifyAspNetCoreConfiguration(IMvcBuilder mvcBuilder)
    {
        MvcBuilder = mvcBuilder;
    }

    public IMvcBuilder MvcBuilder { get; }

    public IChatifyAspNetCoreConfiguration AddController()
    {
        var featureProvider =
            new ControllerRegistrationFeatureProvider([
                typeof(ChatifyAccountController),
                typeof(ChatifyChatController),
                typeof(ChatifyThreadController),
            ]);

        MvcBuilder.ConfigureApplicationPartManager(x => x.FeatureProviders.Add(featureProvider));
        MvcBuilder.Services.AddScoped<IChatifyAspNetCoreService, ChatifyAspNetCoreService>();
        return this;
    }

    public IChatifyAspNetCoreConfiguration AddSignalR<THub>() where THub : ChatifyHub
    {
        MvcBuilder.Services.AddSignalR();
        MvcBuilder.Services
            .AddScoped<IEventSubscriber<IEnumerable<ChatMessageSentEvent>>, ChatifyChatSignalREventSubscriber<THub>>()
            .AddScoped<IEventSubscriber<IEnumerable<NewChatMessageCreatedEvent>>, ChatifyChatSignalREventSubscriber<THub>>()
            .AddScoped<IEventSubscriber<IEnumerable<ChatMessageReadEvent>>, ChatifyChatSignalREventSubscriber<THub>>()
            .AddScoped<IEventSubscriber<IEnumerable<ChatMemberAddedEvent>>, ChatifyChatSignalREventSubscriber<THub>>()
            .AddScoped<IEventSubscriber<IEnumerable<ChatMemberRemovedEvent>>, ChatifyChatSignalREventSubscriber<THub>>()
            .AddScoped<IEventSubscriber<IEnumerable<ChatNameChangedEvent>>, ChatifyChatSignalREventSubscriber<THub>>()
            
            .AddScoped<IEventSubscriber<IEnumerable<ThreadCreatedEvent>>, ChatifyThreadSignalREventSubscriber<THub>>()
            .AddScoped<IEventSubscriber<IEnumerable<ThreadMessageReadEvent>>, ChatifyThreadSignalREventSubscriber<THub>>()
            .AddScoped<IEventSubscriber<IEnumerable<ThreadMessageSendEvent>>, ChatifyThreadSignalREventSubscriber<THub>>()
            .AddScoped<IEventSubscriber<IEnumerable<ThreadWatchAddedEvent>>, ChatifyThreadSignalREventSubscriber<THub>>()
            .AddScoped<IEventSubscriber<IEnumerable<ThreadWatchRemovedEvent>>, ChatifyThreadSignalREventSubscriber<THub>>()
            
            .AddSingleton<IChatifyHubConnections, ChatifyHubConnections>();

        return this;
    }

    public static ChatifyAspNetCoreConfiguration GetOrCreate(IMvcBuilder mvcBuilder)
    {
        mvcBuilder = mvcBuilder ?? throw new ArgumentNullException(nameof(mvcBuilder));

        var instance = (ChatifyAspNetCoreConfiguration?)mvcBuilder.Services.FirstOrDefault(x =>
                x.ImplementationInstance?.GetType() == typeof(ChatifyAspNetCoreConfiguration))
            ?.ImplementationInstance;

        if (instance != null)
            return instance;

        instance = new ChatifyAspNetCoreConfiguration(mvcBuilder);
        mvcBuilder.Services.AddSingleton(instance);
        return instance;
    }

    private class ControllerRegistrationFeatureProvider : IApplicationFeatureProvider<ControllerFeature>
    {
        private readonly Type[] _controllerTypes;

        public ControllerRegistrationFeatureProvider(Type[] controllerTypes)
        {
            _controllerTypes = controllerTypes ?? throw new ArgumentNullException(nameof(controllerTypes));
        }

        public void PopulateFeature(IEnumerable<ApplicationPart> parts, ControllerFeature feature)
        {
            var notRegistered = _controllerTypes.Where(x => !feature.Controllers.Contains(x.GetTypeInfo())).ToArray();

            foreach (var type in notRegistered)
            {
                feature.Controllers.Add(type.GetTypeInfo());
            }
        }
    }
}
