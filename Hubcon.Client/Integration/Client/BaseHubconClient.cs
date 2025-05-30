﻿using Autofac;
using Castle.DynamicProxy;
using Hubcon.Shared.Abstractions.Standard.Interfaces;
using Hubcon.Shared.Abstractions.Interfaces;
using Hubcon.Client.Abstractions.Interfaces;

namespace Hubcon.Client.Integration.Client
{
    public class HubconClientProvider : IHubconClientProvider
    {
        private readonly ILifetimeScope _lifetimeScope;
        private IControllerContract? _client = null!;
        private readonly IContractInterceptor Interceptor;
        private readonly IProxyRegistry _proxyRegistry;
        public ICommunicationHandler Connection { get => Interceptor.CommunicationHandler; }

        public HubconClientProvider(
            IContractInterceptor interceptor,
            IProxyRegistry proxyRegistry,
            ILifetimeScope lifetimeScope) : base()
        {
            Interceptor = interceptor;
            _proxyRegistry = proxyRegistry;
            _lifetimeScope = lifetimeScope;
        }

        public TICommunicationContract GetClient<TICommunicationContract>() where TICommunicationContract : IControllerContract
        {
            if (_client != null)
                return (TICommunicationContract)_client;

            var proxyType = _proxyRegistry.TryGetProxy<TICommunicationContract>();
            _client = (TICommunicationContract)_lifetimeScope.Resolve(proxyType, new[]
            {
                new TypedParameter(typeof(AsyncInterceptorBase), Interceptor)
            });

            return (TICommunicationContract)_client;
        }
    }
}