using System;
using System.Diagnostics;
using System.Threading;

public sealed class WheelRateLimiter
{
    private readonly int[] _counts;
    private readonly int   _limit;
    private readonly int   _slotCount;
    private readonly long  _slotDurationMs;

    private int  _currentSlot;
    private long _lastAdvanceMs;

    private static long GetMs() =>
        Stopwatch.GetTimestamp() * 1000L / Stopwatch.Frequency;

    public WheelRateLimiter(int limit, TimeSpan window, int slotCount = 60)
    {
        _limit          = limit;
        _slotCount      = slotCount;
        _counts         = new int[slotCount];
        _slotDurationMs = (long)(window.TotalMilliseconds / slotCount);
        _lastAdvanceMs  = GetMs();
    }

    public bool TryAcquire(int permits)
    {
        if (permits == 0)
        {
            var spinner = new SpinWait();
            while (true)
            {
                Advance();
                if (CurrentCount() < _limit) return true;
                spinner.SpinOnce();
            }
        }

        Advance();

        Interlocked.Add(ref _counts[_currentSlot], permits);

        if (CurrentCount() <= _limit) return true;

        Interlocked.Add(ref _counts[_currentSlot], -permits);
        return false;
    }
    
    public bool TryAcquire(int permits, CancellationToken ct)
    {
        if (permits > 0) return TryAcquire(permits);

        var spinner = new SpinWait();
        while (!ct.IsCancellationRequested)
        {
            Advance();
            if (CurrentCount() < _limit) return true;
            spinner.SpinOnce();
        }
        return false;
    }

    private void Advance()
    {
        var now        = GetMs();
        var lastAdvance = Volatile.Read(ref _lastAdvanceMs);
        var elapsed    = now - lastAdvance;
        if (elapsed < _slotDurationMs) return;

        // Fix race condition: CAS garantiza que solo 1 thread avanza
        // Si otro thread ganó el CAS primero, este simplemente sale
        if (Interlocked.CompareExchange(ref _lastAdvanceMs, now, lastAdvance) != lastAdvance)
            return;

        var slotsToAdvance = (int)Math.Min(elapsed / _slotDurationMs, _slotCount);

        for (int i = 0; i < slotsToAdvance; i++)
        {
            var next = (_currentSlot + 1) % _slotCount;
            Interlocked.Exchange(ref _counts[next], 0);   // limpia antes de usar
            Interlocked.Exchange(ref _currentSlot, next);
        }
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