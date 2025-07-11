﻿using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;

namespace Hubcon.Server.Core.Configuration
{
    public interface ICoreServerOptions
    {
        /// <summary>
        /// Sets the maximum incoming message size for websockets. Default value is 16384 bytes.
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        ICoreServerOptions SetMaxWebSocketMessageSize(int bytes);

        /// <summary>
        /// Sets the maximum incoming message size for HTTP.
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        ICoreServerOptions SetMaxHttpMessageSize(int bytes);

        /// <summary>
        /// Set the timeout for websocket connections. If the connection remains silent for the specified time, the connection will automatically be closed.
        /// The client should send a ping message to keep the connection alive. Default is 30 seconds.
        /// </summary>
        /// <param name="timeout"></param>
        /// <returns></returns>
        ICoreServerOptions SetWebSocketTimeout(TimeSpan timeout);

        /// <summary>
        /// Set the timeout for HTTP connections. If a request takes longer than the specified time, the operation is cancelled. Default is 15 seconds.
        /// </summary>
        /// <param name="timeout"></param>
        /// <returns></returns>
        ICoreServerOptions SetHttpTimeout(TimeSpan timeout);


        /// <summary>
        /// Determines if the server should send a pong message to the client.
        /// </summary>
        /// <param name="enabled"></param>
        /// <returns></returns>
        ICoreServerOptions DisableWebSocketPong(bool enabled = true);

        /// <summary>
        /// Sets the path prefix that the websocket will be listening on.
        /// </summary>
        /// <param name="prefix"></param>
        /// <returns></returns>
        ICoreServerOptions SetWebSocketPathPrefix(string prefix);

        /// <summary>
        /// Sets the path prefix that the HTTP endpoints will be bound to.
        /// </summary>
        /// <param name="prefix"></param>
        /// <returns></returns>
        ICoreServerOptions SetHttpPathPrefix(string prefix);

        /// <summary>
        /// Determines if websocket ingest methods are allowed.
        /// </summary>
        /// <param name="enabled"></param>
        /// <returns></returns>
        ICoreServerOptions DisableWebSocketIngest(bool disabled = true);

        /// <summary>
        /// Determines if websocket subscriptions methods and ISubscription event handlers are allowed.
        /// </summary>
        /// <param name="enabled"></param>
        /// <returns></returns>
        ICoreServerOptions DisableWebSocketSubscriptions(bool disabled = true);

        /// <summary>
        /// Determines if typical controller methods are allowed through the websocket connection. This enables websocket-based controllers.
        /// </summary>
        /// <param name="enabled"></param>
        /// <returns></returns>
        ICoreServerOptions DisableWebSocketMethods(bool disabled = true);

        /// <summary>
        /// Determines if receiving ping messages from clients is required to keep the websocket connection alive.
        /// </summary>
        /// <param name="enabled"></param>
        /// <returns></returns>
        ICoreServerOptions DisableWebsocketPing(bool disabled = true);

        /// <summary>
        /// Determines if retryable messages should be used when detected.
        /// </summary>
        /// <param name="enabled"></param>
        /// <returns></returns>
        ICoreServerOptions DisabledRetryableMessages(bool enabled = true);

        /// <summary>
        /// Tells the global exception middleware to include detailed error messages in error responses.
        /// </summary>
        /// <param name="enabled"></param>
        /// <returns></returns>
        ICoreServerOptions EnableRequestDetailedErrors(bool enabled = true);
        ICoreServerOptions DisableWebSocketStream(bool disabled = true);
        ICoreServerOptions UseGlobalRouteHandlerBuilder(Action<RouteHandlerBuilder> configure);
        ICoreServerOptions UseGlobalHttpConfigurations(Action<IEndpointConventionBuilder> configure);
    }

    public interface IInternalServerOptions
    {
        /// <summary>
        /// Determines the maximum incoming websocket message size in bytes.
        /// </summary>
        long MaxWebSocketMessageSize { get; }

        /// <summary>
        /// Disabled. Determines the maximum incoming http message size in bytes.
        /// </summary>
        long MaxHttpMessageSize { get; }

        /// <summary>
        /// Websocket connection timeout when the
        /// </summary>
        TimeSpan WebSocketTimeout { get; }

        /// <summary>
        /// Disabled. Http message processing timeout.
        /// </summary>
        TimeSpan HttpTimeout { get; }

        /// <summary>
        /// Determines if clients need to send ping messages to keep the connection alive.
        /// </summary>
        bool WebsocketRequiresPing { get; }

        /// <summary>
        /// Determines if the websocket should handle RetryableMessage.
        /// </summary>
        bool MessageRetryIsEnabled { get; }

        /// <summary>
        /// Determines if "pong" message is sent to the client when a ping message is received.
        /// </summary>
        bool WebSocketPongEnabled { get; }

        /// <summary>
        /// Websocket prefix to bind to.
        /// </summary>
        string WebSocketPathPrefix { get; }

        /// <summary>
        /// HTTP prefix to bind to.
        /// </summary>
        string HttpPathPrefix { get; }

        /// <summary>
        /// Determines if ingest methods are allowed.
        /// </summary>
        bool WebSocketIngestIsAllowed { get; }

        /// <summary>
        /// Determines if subscriptions are allowed.
        /// </summary>
        bool WebSocketSubscriptionIsAllowed { get; }

        /// <summary>
        /// Determines if websocket streams are allowed.
        /// </summary>
        bool WebSocketStreamIsAllowed { get; }

        /// <summary>
        /// Determines if typical controller methods are allowed.
        /// </summary>
        bool WebSocketMethodsIsAllowed { get; }

        /// <summary>
        /// Determines if responses should include detailed error messages.
        /// </summary>
        bool DetailedErrorsEnabled { get; }
        Action<IEndpointConventionBuilder>? EndpointConventions { get; }
        Action<RouteHandlerBuilder>? RouteHandlerBuilderConfig { get; }
    }
}