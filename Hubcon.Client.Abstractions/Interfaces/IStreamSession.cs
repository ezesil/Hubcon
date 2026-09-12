using Hubcon.Shared.Core.Websockets.Messages.Generic;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading;
using Hubcon.Shared.Core.Websockets.Messages.Streams;

namespace Hubcon.Client.Abstractions.Interfaces
{
    public interface IStreamSession : IDisposable
    {
        StreamInitMessage Payload { get; }

        void AddCancellation(Action callback, CancellationToken cancellationToken);
        void AddCancellation(Action<object?> callback, object? state, CancellationToken cancellationToken);
        void Next(JsonElement streamDataData);
        void TryComplete();
    }

    public interface IStreamSession<out T> : IStreamSession
    {
        StreamInitMessage Payload { get; }

        IObservable<T> GetObservable();
    }
}
