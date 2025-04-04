﻿using Hubcon.Core.Interfaces;
using Hubcon.Core.Interfaces.Communication;

namespace Hubcon.Core.Models.Interfaces
{
    public interface IClientManager
    {
        TICommunicationContract GetClient<TICommunicationContract>(string instanceId) where TICommunicationContract : ICommunicationContract;
        List<string> GetAllClients();
        void RemoveClient(string instanceId);
    }

#pragma warning disable S2326 // Unused type parameters should be removed
    public interface IClientManager<out TICommunicationContract, TIHubconController> : IClientManager
#pragma warning restore S2326 // Unused type parameters should be removed
        where TICommunicationContract : ICommunicationContract?
        where TIHubconController : IHubconController
    {
        TICommunicationContract GetOrCreateClient(string instanceId);
    }
}
