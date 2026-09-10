using System;
using System.Threading;
using System.Threading.Tasks;
using Hubcon.Shared.Abstractions.Models;

namespace Hubcon.Shared.Abstractions.Interfaces;

public interface IRateLimitAuthority
{
    bool TryAcquire(RateLimiterKey key, int limit, TimeSpan window, int permits = 1);
    ValueTask<bool> TryAcquireAsync(RateLimiterKey key, int limit, TimeSpan window, int permits = 1, CancellationToken ct = default);
}