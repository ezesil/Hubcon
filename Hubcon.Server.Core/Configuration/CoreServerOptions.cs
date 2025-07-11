﻿using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hubcon.Server.Core.Configuration
{
    public class CoreServerOptions : ICoreServerOptions, IInternalServerOptions
    {
        private long? maxWsSize;
        private long? maxHttpSize;
        private TimeSpan? wsTimeout;
        private TimeSpan? httpTimeout;
        private bool? pongEnabled;
        private string? wsPrefix;
        private string? httpPrefix;

        private bool? allowWsIngest;
        private bool? allowWsSubs;
        private bool? allowWsMethods;
        private bool? websocketRequiresPing;
        private bool? messageRetryIsEnabled;
        private bool? webSocketStreamIsAllowed;
        private bool? detailedErrorsEnabled;
        private Action<IEndpointConventionBuilder>? endpointConventions;
        private Action<RouteHandlerBuilder>? routeHandlerBuilderConfig;

        // Defaults
        public long MaxWebSocketMessageSize => maxWsSize ?? (64 * 1024); // 64 KB
        public long MaxHttpMessageSize => maxHttpSize ?? (128 * 1024);   // 128 KB

        
        public TimeSpan WebSocketTimeout => wsTimeout ?? TimeSpan.FromSeconds(30); 
        public TimeSpan HttpTimeout => httpTimeout ?? TimeSpan.FromSeconds(15);
         
        public string WebSocketPathPrefix => wsPrefix ?? "/ws";
        public string HttpPathPrefix => httpPrefix ?? "";

        public bool WebSocketIngestIsAllowed => allowWsIngest ?? true;
        public bool WebSocketSubscriptionIsAllowed => allowWsSubs ?? true;
        public bool WebSocketStreamIsAllowed => webSocketStreamIsAllowed ?? true;
        public bool WebSocketMethodsIsAllowed => allowWsMethods ?? true;
        public bool WebsocketRequiresPing => websocketRequiresPing ?? true;
        public bool WebSocketPongEnabled => pongEnabled ?? true;
        public bool MessageRetryIsEnabled => messageRetryIsEnabled ?? false;
        public bool DetailedErrorsEnabled => detailedErrorsEnabled ?? false;
        public Action<IEndpointConventionBuilder>? EndpointConventions => endpointConventions;
        public Action<RouteHandlerBuilder>? RouteHandlerBuilderConfig => routeHandlerBuilderConfig;

        public ICoreServerOptions SetMaxWebSocketMessageSize(int bytes)
        {
            maxWsSize ??= bytes;
            return this;
        }

        public ICoreServerOptions SetMaxHttpMessageSize(int bytes)
        {
            maxHttpSize ??= bytes;
            return this;
        }

        public ICoreServerOptions SetWebSocketTimeout(TimeSpan timeout)
        {
            wsTimeout ??= timeout;
            return this;
        }

        public ICoreServerOptions SetHttpTimeout(TimeSpan timeout)
        {
            httpTimeout ??= timeout;
            return this;
        }

        public ICoreServerOptions DisableWebSocketPong(bool disabled = true)
        {
            pongEnabled ??= !disabled;
            return this;
        }

        public ICoreServerOptions SetWebSocketPathPrefix(string prefix)
        {
            wsPrefix ??= "/" + prefix;
            return this;
        }

        public ICoreServerOptions SetHttpPathPrefix(string prefix)
        {
            httpPrefix ??= prefix;
            return this;
        }

        public ICoreServerOptions DisableWebSocketIngest(bool disabled = true)
        {
            allowWsIngest ??= !disabled;
            return this;
        }

        public ICoreServerOptions DisableWebSocketStream(bool disabled = true)
        {
            webSocketStreamIsAllowed ??= !disabled;
            return this;
        }

        public ICoreServerOptions DisableWebSocketSubscriptions(bool disabled = true)
        {
            allowWsSubs ??= !disabled;
            return this;
        }

        public ICoreServerOptions DisableWebSocketMethods(bool disabled = true)
        {
            allowWsMethods ??= !disabled;
            return this;
        }

        public ICoreServerOptions DisableWebsocketPing(bool disabled = true)
        {
            websocketRequiresPing ??= !disabled;
            return this;
        }

        public ICoreServerOptions DisabledRetryableMessages(bool disabled = true)
        {
            messageRetryIsEnabled ??= !disabled;
            return this;
        }

        public ICoreServerOptions EnableRequestDetailedErrors(bool enabled = true)
        {
            detailedErrorsEnabled ??= enabled;
            return this;
        }
        
        public ICoreServerOptions UseGlobalHttpConfigurations(Action<IEndpointConventionBuilder> configure)
        {
            endpointConventions ??= configure;
            return this;
        }

        public ICoreServerOptions UseGlobalRouteHandlerBuilder(Action<RouteHandlerBuilder> configure)
        {
            routeHandlerBuilderConfig ??= configure;
            return this;
        }
    }
}
