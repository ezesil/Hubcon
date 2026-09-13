using System;
using System.Linq;
using System.Reflection;

namespace Hubcon.Shared.Core.Tools;

public static class TypeTools
{
    public static MethodInfo? FindControllerMethod(this Type controllerType, string methodName, Type[] parameterTypes, Type? declaringInterface = null)
    {
        return controllerType
            .GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
            .Where(m =>
            {
                var nameMatch = m.Name == methodName
                                || m.Name.EndsWith($".{methodName}");

                if (!nameMatch) return false;

                if (declaringInterface != null && m.Name.Contains('.'))
                    if (!m.Name.StartsWith(declaringInterface.FullName ?? ""))
                        return false;

                var parameters = m.GetParameters();
                return parameters.Length == parameterTypes.Length
                       && parameters.Select(p => p.ParameterType)
                           .SequenceEqual(parameterTypes);
            })
            .FirstOrDefault();
    }
}