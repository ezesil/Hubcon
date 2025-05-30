﻿using Hubcon.Shared.Abstractions.Interfaces;
using Microsoft.AspNetCore.SignalR.Client;
using System.Text.Json;

namespace Hubcon.SignalR.Extensions
{
    internal static class HubConnectionExtensions
    {
        public static async Task<IAsyncEnumerable<T>> StreamAsync<T>(
            this HubConnection connection, 
            IOperationRequest request,
            IDynamicConverter converter, 
            CancellationToken cancellationToken)
        {
            var stream = connection.StreamAsync<JsonElement>(request.OperationName!, request, cancellationToken);
            return await Task.FromResult(converter.ConvertStream<T>(stream, cancellationToken));
        }
    }
}
