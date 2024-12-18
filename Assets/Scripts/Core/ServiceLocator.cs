using System;
using System.Collections.Generic;

public static class ServiceLocator
{
    private static readonly Dictionary<Type, object> services = new Dictionary<Type, object>();

    public static void RegisterService<T>(T service)
    {
        Type serviceType = typeof(T);
        if (!services.ContainsKey(serviceType))
        {
            services[serviceType] = service;
        }
        else
        {
            //TODO: LOG
        }
    }

    public static T GetService<T>()
    {
        Type serviceType = typeof(T);
        if (services.TryGetValue(serviceType, out var service))
        {
            return (T)service;
        }

        //TODO: LOG

        return default;
    }

    public static void RemoveService<T>()
    {
        Type serviceType = typeof(T);
        if (services.ContainsKey(serviceType))
        {
            services.Remove(serviceType);
        }
    }
}
