﻿using Newtonsoft.Json;
using System.Reflection;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Hubcon.Core.Converters
{
    public class DynamicConverter
    {
        public Dictionary<Delegate, Type[]> TypeCache { get; private set; } = new();

        public object?[] SerializeArgs(object?[] args)
        {
            if(args == null)
                return Array.Empty<object>();
            if (args.Length == 0) 
                return Array.Empty<object>();

            for (int i = 0; i < args.Length; i++)
                args[i] = JsonConvert.SerializeObject(args[i]);

            return args;
        }

        public object?[] DeserializeArgs(Type[] types, object?[] args)
        {
            if (types.Length == 0) return Array.Empty<object>();

            if (types.Length != args.Length)
                throw new ArgumentException("El número de tipos y valores debe coincidir.");

            for (int i = 0; i < types.Length; i++)
            {
                if (typeof(IAsyncEnumerable<object>).IsAssignableFrom(types[i]))
                    args[i] = (IAsyncEnumerable<object>?)args[i];

                args[i] = JsonConvert.DeserializeObject($"{args[i]}", types[i]);
            }

            return args;
        }

        public object?[] DeserializedArgs(Delegate del, object?[] args)
        {
            if (args.Length == 0) return Array.Empty<object>();

            Type[] parameterTypes;

            if (TypeCache.TryGetValue(del, out var types))
            {
                parameterTypes = types;
            }
            else
            {
                parameterTypes = del
                .GetMethodInfo()
                .GetParameters()
                .Where(p => !p.ParameterType.FullName?.Contains("System.Runtime.CompilerServices.Closure") ?? true)
                .Select(p => p.ParameterType)
                .ToArray();
            }

            return DeserializeArgs(parameterTypes, args);
        }

        public string? SerializeData(object? data) => data == null ? null : JsonConvert.SerializeObject(data);    
        public object? DeserializeData(Type type, object data) => data == null ? null : JsonConvert.DeserializeObject($"{data}", type);
        public T? DeserializeData<T>(object? data)
        {
            if (data == null) return default;

            if(typeof(IAsyncEnumerable<object>).IsAssignableFrom(typeof(T)))
                return (T)data;

            return JsonConvert.DeserializeObject<T>($"{data}");       
        }   
    }
}
