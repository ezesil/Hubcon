﻿using Castle.DynamicProxy;
using Hubcon.Extensions;
using Hubcon.Models;
using Microsoft.AspNetCore.SignalR.Client;

namespace Hubcon.Interceptors
{
    internal class ServerHubControllerConnectorInterceptor(HubConnection hub) : AsyncInterceptorBase
    {
        protected override async Task<TResult> InterceptAsync<TResult>(IInvocation invocation, IInvocationProceedInfo proceedInfo, Func<IInvocation, IInvocationProceedInfo, Task<TResult>> proceed)
        {
            if (hub.State != HubConnectionState.Connected) await hub.StartAsync();

            var result = await hub.InvokeServerMethodAsync(invocation.Method.GetMethodSignature(), new CancellationToken(), invocation.Arguments);
                
            // Convertir el resultado y devolverlo
            invocation.ReturnValue = result?.Data;
            return (TResult)result?.Data!;
        }

        protected override async Task InterceptAsync(IInvocation invocation, IInvocationProceedInfo proceedInfo, Func<IInvocation, IInvocationProceedInfo, Task> proceed)
        {
            if (hub.State != HubConnectionState.Connected) await hub.StartAsync();

            await hub
                .CallServerMethodAsync(invocation.Method.GetMethodSignature(), new CancellationToken(), invocation.Arguments);
        }
    }
}