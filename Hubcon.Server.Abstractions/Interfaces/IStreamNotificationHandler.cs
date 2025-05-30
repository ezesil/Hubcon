﻿using Hubcon.Shared.Abstractions.Interfaces;
using System.Threading.Channels;

namespace Hubcon.Server.Abstractions.Interfaces
{
    public interface IStreamNotificationHandler
    {
        Task<IResponse> NotifyStream(string code, ChannelReader<object> reader);
        Task<IAsyncEnumerable<T>> WaitStreamAsync<T>(string code);
    }
}