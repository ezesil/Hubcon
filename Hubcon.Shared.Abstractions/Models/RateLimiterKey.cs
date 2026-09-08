using System;

namespace Hubcon.Shared.Abstractions.Models;

public readonly struct RateLimiterKey : IEquatable<RateLimiterKey>
{
    private readonly string _anchor;
    private readonly int _group;
    private readonly int _transportToken;
    private readonly int _hash;

    public RateLimiterKey(string anchor, int group, int transportToken = 0)
    {
        _anchor = anchor;
        _group = group;
        _transportToken = transportToken;
        _hash = HashCode.Combine(anchor, group, transportToken);
    }

    public bool Equals(RateLimiterKey other) =>
        _group == other._group &&
        _transportToken == other._transportToken &&
        string.Equals(_anchor, other._anchor, StringComparison.Ordinal);

    public override bool Equals(object? obj) => obj is RateLimiterKey k && Equals(k);
    public override int GetHashCode() => _hash;
}