﻿using Hubcon.Core.Converters;
using Hubcon.Core.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Text.Json;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Hubcon.Core.MethodHandling
{
    public class StreamNotificationHandler
    {
        private DynamicConverter _converter;
        private Dictionary<string, IOnStreamReceived> StreamWaitList = new();

        public StreamNotificationHandler(DynamicConverter converter)
        {
            _converter = converter;
        }

        public async Task NotifyStream(string code, ChannelReader<object> reader)
        {
            while (await reader.WaitToReadAsync())
            {
                if (reader.CanPeek)
                {
                    break;
                }
            }

            if (StreamWaitList.TryGetValue(code, out var value))
            {
                value.GetCurrentEvent()?.DynamicInvoke(reader);
            }
        }

        public Task<IAsyncEnumerable<T>> WaitStreamAsync<T>(string code)
        {
            async IAsyncEnumerable<TOut> ToAsyncEnumerable<TOut>(ChannelReader<object> reader)
            {
                await foreach (object item in reader.ReadAllAsync())
                {
                    yield return _converter.DeserializeData<TOut>(item)!;
                }
            }

            var tcs = new TaskCompletionSource<IAsyncEnumerable<T>>();
            var eventHolder = new OnStreamReceived();

            Func<ChannelReader<object>, Task> eventHandler = null!;

            eventHandler = (enumerable) =>
            {
                // Cuando el evento se dispara, resolvemos la tarea
                tcs.SetResult(ToAsyncEnumerable<T>(enumerable));
                // Es importante desuscribirse después de que el evento se ha manejado
                eventHolder.OnStreamReceivedEvent -= eventHandler;

                return Task.CompletedTask;
            };

            StreamWaitList.Add(code, eventHolder);

            eventHolder.OnStreamReceivedEvent += eventHandler;

            // Esperar a que el evento se dispare
            tcs.Task.Wait(5000);

            return tcs.Task;
        }
    }
}
