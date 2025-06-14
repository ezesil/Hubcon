﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hubcon.Shared.Core.Websockets.Heartbeat
{
    public sealed class HeartbeatWatcher : IAsyncDisposable
    {
        private bool timeoutExecuted = false;
        private readonly Func<Task> _onTimeout;
        private readonly int _timeoutSeconds;

        private CancellationTokenSource _cts = new();
        private Task _loop;

        private DateTime _lastHeartbeat = DateTime.UtcNow;

        public HeartbeatWatcher(int timeoutSeconds, Func<Task> onTimeout)
        {
            _timeoutSeconds = timeoutSeconds;
            _onTimeout = onTimeout;

            _loop = RunAsync(_cts.Token);
        }

        public void NotifyHeartbeat()
        {
            _lastHeartbeat = DateTime.UtcNow;
        }

        private async Task RunAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                await Task.Delay(_timeoutSeconds * 1000 / 5, token);

                var elapsed = (DateTime.UtcNow - _lastHeartbeat).TotalSeconds;
                if (elapsed > _timeoutSeconds)
                {
                    if (!timeoutExecuted)
                    {
                        timeoutExecuted = true;
                        await _onTimeout();
                    }
                    break;
                }
            }
        }

        public async ValueTask DisposeAsync()
        {
            _cts.Cancel();
            _cts.Dispose();

            if (!timeoutExecuted)
            {
                timeoutExecuted = true;
                await _onTimeout();
            }

            try { await _loop; } catch { /* swallow */ }
        }
    }

}
