﻿using Hubcon.Client.Abstractions.Interfaces;
using Hubcon.Client.Core.Registries;
using Hubcon.Client.Core.Subscriptions;
using Hubcon.Client.Integration.Client;
using Hubcon.Client.Interceptors;
using Hubcon.Shared.Abstractions.Interfaces;
using Hubcon.Shared.Abstractions.Standard.Interfaces;
using Hubcon.Shared.Core.Attributes;
using Hubcon.Shared.Core.Injection;
using Hubcon.Shared.Core.Serialization;
using Microsoft.Extensions.DependencyInjection;

namespace Hubcon.Client.Builder
{
    public class HubconClientBuilder
    {
        private ProxyRegistry Proxies { get; }
        private ClientBuilderRegistry ClientBuilders { get; }

        private HubconClientBuilder()
        {
            Proxies = new();
            ClientBuilders = new ClientBuilderRegistry(Proxies);

            //var worker2 = new System.Timers.Timer();
            //worker2.Interval = 10000;
            //worker2.Elapsed += (sender, eventArgs) =>
            //{
            //    GC.Collect();
            //    GC.WaitForPendingFinalizers();
            //    GC.Collect();
            //};
            //worker2.AutoReset = true;
            //worker2.Start();
        }

        private static HubconClientBuilder _current = null!;
        public static HubconClientBuilder Current
        {
            get
            {
                _current ??= new HubconClientBuilder();
                return _current;
            }
        }

        public IServiceCollection Services { get; internal set; }

        public IServiceCollection AddHubconClient(IServiceCollection services)
        {
            Services = services;

            services.AddHttpClient();
            services.AddSingleton<IProxyRegistry>(Proxies);
            services.AddSingleton<IClientBuilderRegistry>(ClientBuilders);
            services.AddTransient(typeof(Lazy<>), typeof(LazyResolver<>));
            services.AddSingleton<IDynamicConverter, DynamicConverter>();
            services.AddSingleton<IClientProxyInterceptor, ClientProxyInterceptor>();
            services.AddSingleton<IHubconClient, HubconClient>();
            services.AddTransient(typeof(ClientSubscriptionHandler<>));

            return services;
        }

        public IServiceCollection AddRemoteServerModule<TRemoteServerModule>(IServiceCollection services)
             where TRemoteServerModule : IRemoteServerModule, new()
        {
            ClientBuilders.RegisterModule<TRemoteServerModule>(services);

            return services;
        }

        public void LoadContractProxy(Type contractType, IServiceCollection services)
        {
            if (!contractType.IsAssignableTo(typeof(IControllerContract)))
                return;

            var proxy = GetProxyType(contractType);

            Proxies.RegisterProxy(contractType, proxy);
            services.AddTransient(proxy);
        }

        private static Type? GetProxyType(Type interfaceType)
        {
            // Construir el nombre del proxy basado en la convención
            var proxyTypeName = $"{interfaceType.FullName}Proxy";

            // Buscar en el mismo assembly primero
            var proxyType = interfaceType.Assembly.GetType(proxyTypeName);

            // Si no se encuentra, buscar en todos los assemblies cargados
            if (proxyType == null)
            {
                foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    proxyType = assembly.GetType(proxyTypeName);
                    if (proxyType != null)
                        break;
                }
            }

            return proxyType;
        }

        public IServiceCollection UseContractsFromAssembly(IServiceCollection services, string assemblyName)
        {
            var assembly = AppDomain.CurrentDomain.Load(assemblyName);

            var contracts = assembly
                .GetTypes()
                .Where(t => t.IsInterface && typeof(IControllerContract).IsAssignableFrom(t))
                .ToList();

            var proxies = assembly
                .GetTypes()
                .Where(t => !t.IsInterface && typeof(IControllerContract).IsAssignableFrom(t) && t.IsDefined(typeof(HubconProxyAttribute), inherit:true))
                .ToList();

            foreach (var contract in contracts)
            {
                var proxy = proxies.Find(x => x.Name == contract.Name + "Proxy")!;
                Proxies.RegisterProxy(contract, proxy);
                services.AddScoped(proxy);
            }

            return services;
        }
    }
}