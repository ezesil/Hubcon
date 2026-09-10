using System.Collections.Concurrent;
using Hubcon.Server.Abstractions.Interfaces;
using Hubcon.Server.Core.Configuration;
using Hubcon.Shared.Abstractions.Interfaces;
using Hubcon.Shared.Core.Websockets;
using System.Reflection;
using System.Threading.RateLimiting;
using Hubcon.Shared.Abstractions.Models;

#pragma warning disable CS1591
namespace Hubcon.Server.Core.RateLimiting
{
    public sealed class GlobalRateLimiterManager(
        IOperationCache cache,
        IInternalServerOptions options,
        IOperationConfigRegistry operationConfigRegistry,
        IOperationRegistry operationRegistry,
        IRateLimitAuthority globalAuthority)
        : IGlobalRateLimiterManager
    {
        private readonly SettingsManager _settingsManager =
            new(operationRegistry, operationConfigRegistry);
        
        private IRateLimitAuthority ResolveAuthority(HubconTransportAttribute transport) =>
            options.TransportSettings.TryGetValue(transport, out var authority)
                ? authority.TransportRateLimitAuthority ?? globalAuthority
                : globalAuthority;

        public ValueTask<bool> TryAcquireAsync(
            string anchorKey,
            MessageType type,
            HubconTransportAttribute transport,
            IOperationRequest? operation = null,
            int permits = 1,
            CancellationToken cancellationToken = default)
        {
            if (options.ThrottlingIsDisabled) return new(true);

            try
            {
                var settings = GetTransportSettings(transport);

                if (!settings.UseRateLimiters) 
                    return new ValueTask<bool>(true);
                
                var authority = ResolveAuthority(transport);
                var token = transport.TransportType.MetadataToken;

                // Per transport
                var globalKey = new RateLimiterKey(anchorKey, -1, token);
                if (!authority.TryAcquire(globalKey, settings.TransportLimitPerSecond, TimeSpan.FromSeconds(1), permits))
                    return new(false);

                // Per message type
                var group = GetGroupKey(type);
                if (group >= 0)
                {
                    var typeLimit = GetLimitFromSettings(settings, group);
                    if (typeLimit.HasValue && typeLimit != 0)
                    {
                        var typeKey = new RateLimiterKey(anchorKey, group, token);
                        if (!authority.TryAcquire(typeKey, typeLimit.Value, TimeSpan.FromSeconds(1), permits))
                            return new(false);
                    }
                }
                
                if (operation is null) return new(true);
                
                // Per contract
                if (!TryAcquireContractLimit(anchorKey, operation, transport, authority, settings, permits))
                    return new(false);

                // Per operation
                if (!TryAcquireOperationLimit(anchorKey, operation, transport, authority, settings, permits))
                    return new(false);

                return new(true);
            }
            catch
            {
                return new(false);
            }
        }
        
        public ValueTask<bool> TryAcquireForResourceAsync(
            string anchorKey,
            MessageType type,
            Guid resourceId,
            HubconTransportAttribute transport,
            int permits = 1,
            CancellationToken cancellationToken = default)
        {
            if (options.ThrottlingIsDisabled) return new(true);

            try
            {
                var settings = GetTransportSettings(transport);
                
                if (!settings.UseRateLimiters) 
                    return new ValueTask<bool>(true);
                
                var authority = ResolveAuthority(transport);
                var token = transport.TransportType.MetadataToken;

                // Global
                var globalKey = new RateLimiterKey(anchorKey, -1, token);
                if (!authority.TryAcquire(globalKey, settings.TransportLimitPerSecond, TimeSpan.FromSeconds(1), permits))
                    return new(false);

                // MessageType
                var group = GetGroupKey(type);
                if (group >= 0)
                {
                    var typeLimit = GetLimitFromSettings(settings, group);
                    if (typeLimit.HasValue && typeLimit != 0)
                    {
                        var typeKey = new RateLimiterKey(anchorKey, group, token);
                        if (!authority.TryAcquire(typeKey, typeLimit.Value, TimeSpan.FromSeconds(1), permits))
                            return new(false);
                    }
                }

                // Capa 3 — Guid linked
                if (resourceId != Guid.Empty)
                {
                    var linkedSettings = GetLinkedSettings(type, resourceId);
                    if (linkedSettings != null)
                        return AcquireLinkedAsync(linkedSettings.RateBucket, permits, cancellationToken);
                }

                return new(true);
            }
            catch
            {
                return new(false);
            }
        }

        private bool TryAcquireContractLimit(
            string anchorKey,
            IOperationEndpoint endpoint,
            HubconTransportAttribute transport,
            IRateLimitAuthority authority,
            ITransportSettings settings,
            int permits)
        {
            if (!operationRegistry.TryGetOperationBlueprint(endpoint, transport, out var blueprint))
                return true;

            var attr = blueprint!.ContractType.GetCustomAttribute<RateLimitAttribute>() ??
                       blueprint.ControllerType.GetCustomAttribute<RateLimitAttribute>();

            if (attr is null) return true;

            // Anchor incluye contract name — el blueprint.SimpleContractName es constante,
            // no hay forma de evitar el string aquí sin un struct de 3 strings
            var key = new RateLimiterKey($"{anchorKey}:{blueprint.SimpleContractName}", -2,
                transport.TransportType.MetadataToken);

            return authority.TryAcquire(key, attr.RateTokenLimit, TimeSpan.FromMilliseconds(attr.MillisecondsToReplenish), permits);
        }

        // ── Operation limiter ─────────────────────────────────────────────
        private bool TryAcquireOperationLimit(
            string anchorKey,
            IOperationEndpoint endpoint,
            HubconTransportAttribute transport,
            IRateLimitAuthority authority,
            ITransportSettings settings,
            int permits)
        {
            var attr = _settingsManager.GetSettings<RateLimitAttribute>(endpoint, transport, static () => null!);
            if (attr is null || attr.RateTokenLimit == 0) return true;

            if (!operationRegistry.TryGetOperationBlueprint(endpoint, transport, out var blueprint))
                return true;

            var key = new RateLimiterKey(
                $"{anchorKey}:{blueprint!.SimpleContractName}:{blueprint.OperationName}", -3,
                transport.TransportType.MetadataToken);

            return authority.TryAcquire(key, attr.RateTokenLimit, TimeSpan.FromMilliseconds(attr.MillisecondsToReplenish), permits);
        }

        // ── Helpers ───────────────────────────────────────────────────────
        private ITransportSettings GetTransportSettings(HubconTransportAttribute transport) =>
            options.TransportSettings.TryGetValue(transport, out var s) ? s : transport.DefaultTransportSettings;

        // Int en lugar de string → el switch es un jump table
        private static int GetGroupKey(MessageType type) => type switch
        {
            MessageType.operation_invoke => 0,
            MessageType.operation_call => 1,
            MessageType.ping => 2,
            MessageType.stream_init or MessageType.stream_complete
                or MessageType.stream_data or MessageType.stream_data_ack
                or MessageType.stream_data_with_ack => 3,
            MessageType.ingest_init or MessageType.ingest_data
                or MessageType.ingest_data_with_ack or MessageType.ingest_complete
                or MessageType.ingest_result => 4,
            MessageType.token_update => 5,
            _ => -1
        };

        private static int? GetLimitFromSettings(ITransportSettings settings, int group) => group switch
        {
            0 => settings.InvokeOperationLimitPerSecond,
            1 => settings.CallOperationLimitPerSecond,
            2 => settings.PingOperationLimitPerSecond,
            3 => settings.StreamOperationLimitPerSecond,
            4 => settings.IngestOperationLimitPerSecond,
            5 => settings.ControlMessagesPerSecond,
            _ => null,
        };

        private static async ValueTask<bool> AcquireLinkedAsync(RateLimiter bucket, int permits, CancellationToken ct)
            => (await bucket.AcquireAsync(permits, ct)).IsAcquired;

        public ValueTask Link(string anchorKey, Guid id, HubconTransportAttribute transportAttribute, IOperationRequest request)
        {
            if (!operationRegistry.TryGetOperationBlueprint(request, transportAttribute, out var blueprint))
                return new();

            var state = (registry: operationConfigRegistry, id);
            
            operationConfigRegistry.Link(id, blueprint!);
            cache.Set($"link_{anchorKey}_{id}", request, () => state.registry.Unlink(state.id));

            return new();
        }

        public ValueTask Unlink(string anchorKey, Guid operationId)
        {
            operationConfigRegistry.Unlink(operationId);
            cache.Remove($"link_{anchorKey}_{operationId}");
            return new();
        }

        private RateLimitAttribute? GetLinkedSettings(MessageType type, Guid id) => type switch
        {
            MessageType.connection_ack or MessageType.connection_init
                or MessageType.pong or MessageType.error or MessageType.ack
                or MessageType.ingest_init_ack or MessageType.ingest_data_ack
                or MessageType.operation_response or MessageType.ping => null,

            MessageType.operation_invoke or MessageType.operation_call
                => _settingsManager.GetSettings(id, static () => new RateLimitAttribute()),

            MessageType.subscription_init or MessageType.subscription_data
                or MessageType.subscription_data_with_ack or MessageType.subscription_complete
                => _settingsManager.GetSettings(id, static () => new RateLimitAttribute()),

            MessageType.stream_init or MessageType.stream_complete
                or MessageType.stream_data or MessageType.stream_data_ack
                or MessageType.stream_data_with_ack
                => _settingsManager.GetSettings(id, static () => new RateLimitAttribute()),

            MessageType.ingest_init or MessageType.ingest_data
                or MessageType.ingest_data_with_ack or MessageType.ingest_complete
                or MessageType.ingest_result
                => _settingsManager.GetSettings(id, static () => new RateLimitAttribute()),

            _ => null,
        };
    }
}