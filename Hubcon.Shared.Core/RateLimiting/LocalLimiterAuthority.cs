using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Hubcon.Shared.Abstractions.Interfaces;
using Hubcon.Shared.Abstractions.Models;

namespace Hubcon.Server.Core.RateLimiting;

public sealed class LocalLimiterAuthority : IRateLimitAuthority
{
    private readonly ConcurrentDictionary<RateLimiterKey, WheelRateLimiter> _wheels = new();

    public bool TryAcquire(RateLimiterKey key, int limit, TimeSpan window, int permits = 1)
    {
        var wheel = _wheels.GetOrAdd(key,
            static (k, args) => new WheelRateLimiter(args.limit, args.window),
            (limit, window));
        return wheel.TryAcquire(permits);
    }
    
    public ValueTask<bool> TryAcquireAsync(RateLimiterKey key, int limit, TimeSpan window, int permits = 1, CancellationToken ct = default)
    {
        var wheel = _wheels.GetOrAdd(key,
            static (k, args) => new WheelRateLimiter(args.limit, args.window),
            (limit, window));
        
        return new(wheel.TryAcquire(permits, ct));
    }
}