﻿using System.Reflection;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Webinex.Chatify.AspNetCore.Controller;
using Webinex.Chatify.AspNetCore.SignalR;
using Webinex.Chatify.Common.Events;
using Webinex.Chatify.Types.Events;

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
            new ControllerRegistrationFeatureProvider(typeof(ChatifyController));
        MvcBuilder.ConfigureApplicationPartManager(x => x.FeatureProviders.Add(featureProvider));
        return this;
    }

    public IChatifyAspNetCoreConfiguration AddSignalR<THub>() where THub : ChatifyHub
    {
        MvcBuilder.Services.AddSignalR();
        MvcBuilder.Services
            .AddScoped<ISubscriber<IEnumerable<DeliveryCreatedEvent>>, ChatifySignalREventHandler<THub>>()
            .AddScoped<ISubscriber<IEnumerable<ReadEvent>>, ChatifySignalREventHandler<THub>>()
            .AddScoped<ISubscriber<IEnumerable<MemberAddedEvent>>, ChatifySignalREventHandler<THub>>()
            .AddScoped<ISubscriber<IEnumerable<MemberRemovedEvent>>, ChatifySignalREventHandler<THub>>()
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
        private readonly Type _controllerType;

        public ControllerRegistrationFeatureProvider(Type controllerType)
        {
            _controllerType = controllerType ?? throw new ArgumentNullException(nameof(controllerType));
        }

        public void PopulateFeature(IEnumerable<ApplicationPart> parts, ControllerFeature feature)
        {
            if (feature.Controllers.Contains(_controllerType.GetTypeInfo()))
                return;

            feature.Controllers.Add(_controllerType.GetTypeInfo());
        }
    }
}