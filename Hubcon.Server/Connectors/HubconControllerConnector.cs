﻿using Hubcon.Shared.Abstractions.Standard.Interfaces;
using System.ComponentModel;

namespace Hubcon.Server.Connectors
{
    //[EditorBrowsable(EditorBrowsableState.Never)]
    //public class HubconClientBuilder<TICommunicationContract>
    //    where TICommunicationContract : IControllerContract
    //{
    //    [System.Diagnostics.CodeAnalysis.SuppressMessage(
    //       "Major Code Smell",
    //       "S2743:Static fields should not be used in generic types",
    //       Justification = "The static field by T type is intended.")]
    //    protected Dictionary<string, IOperationRequest> AvailableMethods { get; } = new();

    //    protected HubconClientBuilder()
    //    {
    //        BuildMethods();
    //    }

    //    protected void BuildMethods()
    //    {
    //        if (AvailableMethods.Count == 0)
    //        {
    //            var TType = typeof(TICommunicationContract);

    //            if (!TType.IsInterface)
    //                throw new ArgumentException($"El tipo {typeof(TICommunicationContract).FullName} no es una interfaz.");

    //            if (!typeof(IControllerContract).IsAssignableFrom(TType))
    //                throw new NotImplementedException($"El tipo {TType.FullName} no implementa la interfaz {nameof(IControllerContract)} ni es un tipo derivado.");

    //            foreach (var method in TType.GetMethods())
    //            {
    //                var parameters = method.GetParameters();
    //                var methodSignature = method.GetMethodSignature();
    //                AvailableMethods.TryAdd(methodSignature, new OperationRequest(methodSignature, TType.Name, new DynamicConverter().SerializeArgsToJson(parameters)));
    //            }
    //        }
    //    }
    //}
}
