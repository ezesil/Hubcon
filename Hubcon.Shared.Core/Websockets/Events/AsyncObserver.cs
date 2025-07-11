﻿using System.Runtime.CompilerServices;
using System.Threading.Channels;

namespace Hubcon.Shared.Core.Websockets.Events
{
    public class AsyncObserver<T> : IObserver<T>
    {
        private readonly Channel<T?> _channel = Channel.CreateUnbounded<T?>();
        private TaskCompletionSource<bool> _completed = new TaskCompletionSource<bool>();

        public void WriteToChannelAsync(T? item)
        {
            _ = _channel.Writer.TryWrite(item);
        }

        public IAsyncEnumerable<T?> GetAsyncEnumerable(CancellationToken cancellationToken)
        {
            try
            {
                return ReadAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                OnError(ex);
                throw;
            }
        }

        private async IAsyncEnumerable<T?> ReadAsync([EnumeratorCancellation] CancellationToken cancellationToken)
        {
            try
            {
                await foreach (var item in _channel.Reader.ReadAllAsync(cancellationToken))
                {
                    yield return item;
                }
            }
            finally
            {

            }
        }

        public void OnCompleted()
        {
            _channel.Writer.Complete();
            _completed.SetResult(true);
        }

        public void OnError(Exception error)
        {
            _channel.Writer.Complete(error);
            _completed.SetException(error);
        }

        public void OnNext(T value)
        {
            // Enviar el valor al canal
            WriteToChannelAsync(value);
        }

        public Task WaitUntilCompleted()
        {
            return _completed.Task;
        }
    }
}