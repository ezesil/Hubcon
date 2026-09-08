using System;
using System.Threading.RateLimiting;
using Microsoft.IdentityModel.Tokens;

namespace Hubcon
{
    /// <inheritdoc cref="ITransportSettings" />
    public class TransportSettings : ITransportSettings, ITransportSettingsSetter
    {
        /// <inheritdoc cref="ITransportSettings.MaxMessageSizeInBytes" />
        public virtual long MaxMessageSizeInBytes { get; set; } = 65535;

        /// <inheritdoc cref="ITransportSettings.RequestTimeout" />
        public virtual TimeSpan RequestTimeout { get; set; } = TimeSpan.FromSeconds(15);

        /// <inheritdoc cref="ITransportSettings.MaxConnections" />
        public virtual int MaxConnections { get; set; } = 1000;

        /// <inheritdoc cref="ITransportSettings.MaxConnectionsPerIp" />
        public virtual int MaxConnectionsPerIp { get; set; } = 10;

        /// <inheritdoc cref="ITransportSettings.EnablePing" />
        public virtual bool EnablePing { get; set; } = true;

        /// <inheritdoc cref="ITransportSettings.PingOperationLimitPerSecond" />
        public int? PingOperationLimitPerSecond { get; set; }

        /// <inheritdoc cref="ITransportSettings.EnablePong" />
        public virtual bool EnablePong { get; set; } = true;

        /// <inheritdoc cref="ITransportSettings.TransportPrefix" />
        public virtual string TransportPrefix { get; set; } = "/";

        /// <inheritdoc cref="ITransportSettings.CallOperationEnabled" />
        public virtual bool CallOperationEnabled { get; set; } = true;

        /// <inheritdoc cref="ITransportSettings.CallOperationTimeout" />
        public virtual TimeSpan CallOperationTimeout { get; set; }

        /// <inheritdoc cref="ITransportSettings.CallOperationLimitPerSecond" />
        public int? CallOperationLimitPerSecond { get; set; }

        /// <inheritdoc cref="ITransportSettings.InvokeOperationEnabled" />
        public virtual bool InvokeOperationEnabled { get; set; } = true;

        /// <inheritdoc cref="ITransportSettings.InvokeOperationTimeout" />
        public virtual TimeSpan InvokeOperationTimeout { get; set; }

        /// <inheritdoc cref="ITransportSettings.InvokeOperationLimitPerSecond" />
        public int? InvokeOperationLimitPerSecond { get; set; }

        /// <inheritdoc cref="ITransportSettings.StreamOperationEnabled" />
        public virtual bool StreamOperationEnabled { get; set; } = true;

        /// <inheritdoc cref="ITransportSettings.StreamOperationTimeout" />
        public virtual TimeSpan StreamOperationTimeout { get; set; }

        /// <inheritdoc cref="ITransportSettings.StreamOperationLimitPerSecond" />
        public int? StreamOperationLimitPerSecond { get; set; }

        /// <inheritdoc cref="ITransportSettings.IngestOperationEnabled" />
        public virtual bool IngestOperationEnabled { get; set; } = true;

        /// <inheritdoc cref="ITransportSettings.IngestOperationTimeout" />
        public virtual TimeSpan IngestOperationTimeout { get; set; }

        /// <inheritdoc cref="ITransportSettings.IngestOperationLimitPerSecond" />
        public int? IngestOperationLimitPerSecond { get; set; }
        
        /// <inheritdoc cref="ITransportSettings.ControlMessagesLimitPerSecond" />
        public int? ControlMessagesLimitPerSecond { get; set; }
        
        /// <inheritdoc cref="ITransportSettings.ControlMessagesPerSecond" />
        public int? ControlMessagesPerSecond { get; set; }

        /// <inheritdoc cref="ITransportSettings.RetryableMessagesEnabled" />
        public virtual bool RetryableMessagesEnabled { get; set; }

        /// <inheritdoc cref="ITransportSettings.UseRateLimiters" />
        public virtual bool UseRateLimiters { get; set; } = true;

        /// <inheritdoc cref="ITransportSettings.LoggingEnabled" />
        public virtual bool LoggingEnabled { get; set; }

        /// <inheritdoc cref="ITransportSettings.AllowRemoteCancellation" />
        public virtual bool AllowRemoteCancellation { get; set; }
        
        /// <inheritdoc cref="ITransportSettings.TransportLimitPerSecond" />
        public int? TransportLimitPerSecond { get; set; }

        /// <inheritdoc cref="ITransportSettings.MethodOverloadingEnabled" />
        public virtual bool MethodOverloadingEnabled { get; set; }

        /// <inheritdoc cref="ITransportSettings.MaxConcurrentRequestsPerIp" />
        public virtual int MaxConcurrentRequestsPerIp { get; set; } = 10;

        /// <inheritdoc cref="ITransportSettings.AllowAnonymousClients" />
        public virtual bool AllowAnonymousClients { get; set; } = true;

        /// <inheritdoc cref="ITransportSettings.TokenValidationParameters" />
        public virtual TokenValidationParameters? TokenValidationParameters { get; set; }

        /// <inheritdoc cref="ITransportSettings.CheckTokenExpirationOnMessageReceived" />
        public virtual bool CheckTokenExpirationOnMessageReceived { get; set; }

        /// <inheritdoc cref="ITransportSettings.ConnectionAuthHandlerType" />
        public virtual Type? ConnectionAuthHandlerType { get; set; }

        /// <inheritdoc cref="ITransportSettings.ConnectionTimeout" />
        public virtual TimeSpan ConnectionTimeout { get; set; }

        /// <inheritdoc cref="ITransportSettings.RequiresAuth" />
        public virtual bool RequiresAuth { get; set; } = true;
    }
}