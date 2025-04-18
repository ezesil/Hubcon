﻿using Hubcon.Core.Converters;
using Hubcon.Core.Extensions;
using Hubcon.Core.MethodHandling;
using Hubcon.Core.Middleware;
using Hubcon.Core.Models;
using Hubcon.Core.Models.Interfaces;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;

namespace Hubcon.Core.Handlers
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class RequestPipeline : IRequestPipeline
    {
        private readonly MethodInvokerProvider _methodInvokerProvider;
        private readonly DynamicConverter _converter;
        private readonly IMiddlewareProvider _middlewareProvider;

        public RequestPipeline(MethodInvokerProvider methodInvokerProvider, DynamicConverter dynamicConverter, IMiddlewareProvider middlewareProvider)
        {
            _methodInvokerProvider = methodInvokerProvider;
            _converter = dynamicConverter;
            _middlewareProvider = middlewareProvider;
        }

        public async Task HandleWithoutResultAsync(object instance, MethodInvokeRequest methodInfo)
        {            
            Func<Task<MethodResponse?>> method = async () =>
            {
                _methodInvokerProvider.GetMethodInvoker(methodInfo.MethodName, instance.GetType(), out var methodInvoker);
                var args = methodInfo.GetDeserializedArgs(methodInvoker!.ParameterTypes, _converter.DeserializeArgs);
                object? result = methodInvoker?.Method?.DynamicInvoke(instance, args);

                if (result is Task task)
                    await task;

                return new MethodResponse(true);
            };

            var pipeline = _middlewareProvider.GetPipeline(instance.GetType(), methodInfo, method);
            await pipeline.Execute();
        }

        public async Task<MethodResponse> HandleSynchronousResult(object instance, MethodInvokeRequest methodInfo)
        {
            var method = async () =>
            {
                return await Task.Run(() =>
                {
                    _methodInvokerProvider.GetMethodInvoker(methodInfo.MethodName, instance.GetType(), out var methodInvoker);
                    object?[] args = methodInfo.GetDeserializedArgs(methodInvoker!.ParameterTypes, _converter.DeserializeArgs);
                    object? result = methodInvoker?.Method?.DynamicInvoke(instance, args);

                    if (result is null)
                        return new MethodResponse(true);

                    return new MethodResponse(true, result).SerializeData(_converter.SerializeData);
                });
            };

            var pipeline = _middlewareProvider.GetPipeline(instance.GetType(), methodInfo, method);
            return await pipeline.Execute();
        }

        public Task HandleSynchronous(object instance, MethodInvokeRequest methodInfo)
        {
            Func<Task<MethodResponse?>> method = () =>
            {
                return Task.Run<MethodResponse?>(async () =>
                {
                    _methodInvokerProvider.GetMethodInvoker(methodInfo.MethodName, instance.GetType(), out var methodInvoker);
                    object?[] args = methodInfo.GetDeserializedArgs(methodInvoker!.ParameterTypes, _converter.DeserializeArgs);
                    object? result = methodInvoker?.Method?.DynamicInvoke(instance, args);

                    if (result is Task task)
                        await task;

                    return new MethodResponse(true);
                });
            };

            var pipeline = _middlewareProvider.GetPipeline(instance.GetType(), methodInfo, method);
            return pipeline.Execute();
        }

        public IAsyncEnumerable<object> GetStream(object instance, MethodInvokeRequest methodInfo)
        {
            Func<Task<MethodResponse?>> method = async () =>
            {
                _methodInvokerProvider.GetMethodInvoker(methodInfo.MethodName, instance.GetType(), out var methodInvoker);
                object?[] args = methodInfo.GetDeserializedArgs(methodInvoker!.ParameterTypes, _converter.DeserializeArgs);
                object? result = methodInvoker?.Method?.DynamicInvoke(instance, args);

                return await Task.FromResult(new MethodResponse(true, result!));
            };

            var pipeline = _middlewareProvider.GetPipeline(instance.GetType(), methodInfo, method);
            return (IAsyncEnumerable<object>)pipeline.Execute().Result.Data!;        
        }

        public async Task<MethodResponse> HandleWithResultAsync(object instance, MethodInvokeRequest methodInfo)
        {
            var method = async () =>
            {
                _methodInvokerProvider.GetMethodInvoker(methodInfo.MethodName, instance.GetType(), out var methodInvoker);
                object?[] args = methodInfo.GetDeserializedArgs(methodInvoker!.ParameterTypes, _converter.DeserializeArgs);
                object? result = methodInvoker?.Method?.DynamicInvoke(instance, args);

                if (result is null)
                    return new MethodResponse(true);
                else if (result is Task task)
                {
                    var response = await GetTaskResultAsync(task, methodInvoker!.ReturnType.GetGenericArguments()[0]);
                    return new MethodResponse(true, response).SerializeData(_converter.SerializeData);
                }
                else
                    return new MethodResponse(true, result).SerializeData(_converter.SerializeData);
            };

            var pipeline = _middlewareProvider.GetPipeline(instance.GetType(), methodInfo, method);
            return await pipeline.Execute();
        }

        public static async Task<object?> GetTaskResultAsync(Task taskObject, Type returnType)
        {
            // Esperar a que el Task termine
            await taskObject;

            // Verificar si es un Task<T> (Task con resultado)
            var taskType = taskObject.GetType();

            if (taskType.IsGenericType)
            {
                // Obtener el tipo del resultado (T)
                var resultProperty = taskType.GetProperty("Result");

                // Obtener el resultado del Task
                var result = resultProperty?.GetValue(taskObject);

                return Convert.ChangeType(result, returnType);
            }

            // Si no es un Task<T>, no hay valor que devolver
            return null;
        }

        public void RegisterMethods(Type type, Action<string, MethodInfo>? forEachMethodAction = null) => _methodInvokerProvider.RegisterMethods(type, forEachMethodAction);
    }
}
