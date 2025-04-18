﻿using Autofac;
using Hubcon.Core;
using Hubcon.Core.Controllers;
using Hubcon.Core.Models.Interfaces;
using Hubcon.SignalR.HubActivator;
using Hubcon.SignalR.Server;
using MessagePack;
using MessagePack.Resolvers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;

namespace Hubcon.SignalR
{
    public static class DependencyInjection
    {
        public static WebApplicationBuilder? UseHubconSignalR(this WebApplicationBuilder e)
        {
            MessagePackSerializerOptions mpOptions = MessagePackSerializerOptions.Standard
                .WithResolver(CompositeResolver.Create(
                    ContractlessStandardResolver.Instance
                ));

            e.Services.AddSignalR()
                .AddMessagePackProtocol(options =>
                 {
                     options.SerializerOptions = mpOptions;
                 });

            e.AddHubcon(container =>
            {
                var commHandlerType = typeof(SignalRServerCommunicationHandler<>);
                var hubControllerType = typeof(HubconControllerManager<>);

                container
                    .RegisterWithInjector(x => x.RegisterGeneric(commHandlerType).AsScoped())
                    .RegisterWithInjector(x => x.RegisterGeneric(hubControllerType).As(typeof(IHubconControllerManager<>)).AsScoped())
                    .RegisterWithInjector(x => x.RegisterType(typeof(HubConnectionBuilder)).AsScoped())
                    .RegisterWithInjector(x => x.RegisterGeneric(typeof(HubconHubActivator<>)).As(typeof(IHubActivator<>)).AsScoped());
            });

            return e;
        }
    }
}
