using System;
using System.Diagnostics;
using System.Threading;

namespace Hubcon.Server.Core.RateLimiting;

public sealed class WheelRateLimiter
{
    private readonly int[] _counts;
    private readonly int _limit;
    private readonly int _slotCount;
    private readonly long _slotDurationMs;

    private int _currentSlot;
    private long _lastAdvanceMs;

    private static long GetMs() =>
        Stopwatch.GetTimestamp() * 1000L / Stopwatch.Frequency;

    public WheelRateLimiter(int limit, TimeSpan window, int slotCount = 60)
    {
        _limit = limit;
        _slotCount = slotCount;
        _counts = new int[slotCount];
        _slotDurationMs = (long)(window.TotalMilliseconds / slotCount);
        _lastAdvanceMs = GetMs();
    }

    public bool TryAcquire(int permits = 1)
    {
        Advance();

        var current = Interlocked.Add(ref _counts[_currentSlot], permits);
        if (current <= _limit) return true;

        Interlocked.Add(ref _counts[_currentSlot], -permits); // revert
        return false;
    }

    private void Advance()
    {
        var now = GetMs();
        var elapsed = now - Volatile.Read(ref _lastAdvanceMs);
        if (elapsed < _slotDurationMs) return;

        var slotsToAdvance = (int)Math.Min(elapsed / _slotDurationMs, _slotCount);

        for (int i = 0; i < slotsToAdvance; i++)
        {
            var next = (_currentSlot + 1) % _slotCount;
            Interlocked.Exchange(ref _counts[next], 0);
            Interlocked.Exchange(ref _currentSlot, next);
        }

        Interlocked.Exchange(ref _lastAdvanceMs, now);
    }

    public int CurrentCount()
    {
        int total = 0;
        foreach (var c in _counts.AsSpan()) total += c;
        return total;
    }

    public ReadOnlySpan<int> ExportState() => _counts.AsSpan();

    public void ImportState(ReadOnlySpan<int> state)
    {
        for (int i = 0; i < Math.Min(state.Length, _counts.Length); i++)
            Interlocked.Exchange(ref _counts[i], state[i]);
    }
}