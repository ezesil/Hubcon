﻿namespace Hubcon.SignalR
{
    public static class DependencyInjection
    {
        //public static WebApplicationBuilder? UseHubconSignalR(this WebApplicationBuilder e)
        //{
        //    e.Services.AddSignalR();

        //    e.AddHubconServer(container =>
        //    {
        //        var commHandlerType = typeof(SignalRServerCommunicationHandler<>);
        //        var hubControllerType = typeof(HubconControllerManager);

        //        container
        //            .RegisterWithInjector(x => x.RegisterGeneric(commHandlerType).AsScoped())
        //            .RegisterWithInjector(x => x.RegisterType<HubconControllerManager>().As<IHubconControllerManager>().AsScoped())
        //            .RegisterWithInjector(x => x.RegisterType(typeof(HubConnectionBuilder)).AsScoped())
        //            .RegisterWithInjector(x => x.RegisterGeneric(typeof(HubconHubActivator<>)).As(typeof(IHubActivator<>)).AsScoped());
        //    });

        //    return e;
        //}
    }
}
