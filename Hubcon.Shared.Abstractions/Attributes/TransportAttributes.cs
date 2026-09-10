using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.RateLimiting;
using Microsoft.IdentityModel.Tokens;

namespace Hubcon
{
    /// <summary>
    /// Use Hubcon's HTTP transport implementation. Should be used in the shared contract/interface for the client to adapt automatically. 
    /// <br/> <br/> Note that HTTP is unable to support Ingest operations due to transport limitations and it will throw an exception.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Method |
                    AttributeTargets.Property)]
    public class HttpTransport : HubconTransportAttribute<HttpTransportSettings>
    {
        /// <inheritdoc/>
        public override string TransportKey => "Http";

        /// <inheritdoc/>
        public override int TelemetryId => 0;
    }

    /// <inheritdoc/>
    public class HttpTransportSettings : TransportSettings
    {
        /// <inheritdoc />
        public override TimeSpan RequestTimeout { get; set; } = TimeSpan.FromSeconds(15);

        /// <inheritdoc />
        public override int MaxConnections { get; set; } = 1000;

        /// <inheritdoc />
        public override int MaxConnectionsPerIp { get; set; } = 10;

        /// <inheritdoc />
        public override bool EnablePing { get; set; } = true;

        /// <inheritdoc />
        public override bool EnablePong { get; set; } = true;

        /// <inheritdoc />
        public override string TransportPrefix { get; set; } = "/";

        /// <inheritdoc />
        public override bool CallOperationEnabled { get; set; } = true;

        /// <inheritdoc />
        public override TimeSpan CallOperationTimeout { get; set; }

        /// <inheritdoc />
        public override bool InvokeOperationEnabled { get; set; } = true;

        /// <inheritdoc />
        public override TimeSpan InvokeOperationTimeout { get; set; }

        /// <inheritdoc />
        public override bool StreamOperationEnabled { get; set; } = true;

        /// <inheritdoc />
        public override TimeSpan StreamOperationTimeout { get; set; }

        /// <inheritdoc />
        public override bool IngestOperationEnabled { get; set; } = true;

        /// <inheritdoc />
        public override TimeSpan IngestOperationTimeout { get; set; }

        /// <inheritdoc />
        public override bool RetryableMessagesEnabled { get; set; }

        /// <inheritdoc />
        public override bool UseRateLimiters { get; set; } = true;

        /// <inheritdoc />
        public override bool LoggingEnabled { get; set; }

        /// <inheritdoc />
        public override bool AllowRemoteCancellation { get; set; }

        /// <inheritdoc />
        public override bool MethodOverloadingEnabled { get; set; }

        /// <inheritdoc />
        public override int MaxConcurrentRequestsPerIp { get; set; } = 10;

        /// <inheritdoc />
        public override bool AllowAnonymousClients { get; set; } = true;

        /// <inheritdoc />
        public override TokenValidationParameters? TokenValidationParameters { get; set; }

        /// <inheritdoc />
        public override bool CheckTokenExpirationOnMessageReceived { get; set; }

        /// <inheritdoc />
        public override Type? ConnectionAuthHandlerType { get; set; }

        /// <inheritdoc />
        public override TimeSpan ConnectionTimeout { get; set; }

        /// <inheritdoc />
        public override bool RequiresAuth { get; set; } = true;
    }

    /// <summary>
    /// Use Hubcon's WebSocket transport implementation. Should be used in the shared contract/interface for the client to adapt automatically.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Method |
                    AttributeTargets.Property)]
    public sealed class WebSocketTransport : HubconTransportAttribute<WebSocketTransportSettings>
    {
        /// <inheritdoc/>
        public override string TransportKey => "WebSocket";

        /// <inheritdoc/>
        public override int TelemetryId => 1;
    }

    /// <inheritdoc/>
    public class WebSocketTransportSettings : TransportSettings
    {
        /// <inheritdoc/>
        public override long MaxMessageSizeInBytes { get; set; } = 65535;

        /// <inheritdoc/>w
        public override TimeSpan RequestTimeout { get; set; } = TimeSpan.FromSeconds(30);

        /// <inheritdoc/>
        public override int MaxConnections { get; set; } = 5000;

        /// <inheritdoc/>
        public override int MaxConnectionsPerIp { get; set; } = 25;

        /// <inheritdoc/>
        public override bool EnablePing { get; set; } = true;

        /// <inheritdoc/>
        public override bool EnablePong { get; set; } = true;

        /// <inheritdoc/>
        public override string TransportPrefix { get; set; } = "/ws";

        /// <inheritdoc/>
        public override bool CallOperationEnabled { get; set; } = true;

        /// <inheritdoc/>
        public override TimeSpan CallOperationTimeout { get; set; } = TimeSpan.FromSeconds(10);

        /// <inheritdoc/>
        public override bool InvokeOperationEnabled { get; set; } = true;

        /// <inheritdoc/>
        public override TimeSpan InvokeOperationTimeout { get; set; } = TimeSpan.FromSeconds(30);

        /// <inheritdoc/>
        public override bool StreamOperationEnabled { get; set; } = true;

        /// <inheritdoc/>
        public override TimeSpan StreamOperationTimeout { get; set; } = TimeSpan.FromSeconds(30);

        /// <inheritdoc/>
        public override bool IngestOperationEnabled { get; set; } = true;

        /// <inheritdoc/>
        public override TimeSpan IngestOperationTimeout { get; set; } = TimeSpan.FromSeconds(30);

        /// <inheritdoc/>
        public override bool RetryableMessagesEnabled { get; set; }

        /// <inheritdoc/>
        public override bool UseRateLimiters { get; set; } = true;

        /// <inheritdoc/>
        public override bool LoggingEnabled { get; set; }

        /// <inheritdoc/>
        public override bool AllowRemoteCancellation { get; set; }

        /// <inheritdoc/>
        public override bool MethodOverloadingEnabled { get; set; } = true;

        /// <inheritdoc/>
        public override int MaxConcurrentRequestsPerIp { get; set; } = 25;

        /// <inheritdoc/>
        public override bool AllowAnonymousClients { get; set; } = true;

        /// <inheritdoc/>
        public override TokenValidationParameters? TokenValidationParameters { get; set; }

        /// <inheritdoc/>
        public override bool CheckTokenExpirationOnMessageReceived { get; set; } = true;

        /// <inheritdoc/>
        public override Type? ConnectionAuthHandlerType { get; set; }

        /// <inheritdoc/>
        public override TimeSpan ConnectionTimeout { get; set; } = TimeSpan.FromMinutes(60);

        /// <inheritdoc/>
        public override bool RequiresAuth { get; set; } = true;

        /// <summary>
        /// Determines the heartbeat expiration seconds. If the connection does not receive a ping in time, it may be aborted.
        /// </summary>
        public int HeartBeatInSeconds { get; set; } = 90;
    }

    /// <summary>
    /// Use Non-Hubcon HTTP transport implementation. Should be used in the shared contract/interface for the client to adapt automatically.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Method |
                    AttributeTargets.Property)]
    public sealed class NonHubconHttpTransport : HubconTransportAttribute
    {
        /// <inheritdoc/>
        public override string TransportKey => "NonHubconHttp";

        /// <inheritdoc/>
        public override int TelemetryId => 2;
    }
}