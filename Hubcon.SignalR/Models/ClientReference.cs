﻿using Hubcon.Server.Abstractions.Interfaces;
using Hubcon.Shared.Abstractions.Standard.Interfaces;

namespace Hubcon.SignalR.Models
{
    public class ClientReference : IClientReference
    {
        public string Id { get; }
        public object? ClientInfo { get; set; }

        public ClientReference(string id)
        {
            Id = id;
        }

        public ClientReference(string id, object? clientInfo)
        {
            Id = id;
            ClientInfo = clientInfo;
        }

        public IClientReference<TICommunicationContract> WithController<TICommunicationContract>(TICommunicationContract clientController) where TICommunicationContract : IControllerContract
        {
            return new ClientReference<TICommunicationContract>(this, clientController);
        }
    }
    public class ClientReference<TICommunicationContract> : ClientReference, IClientReference<TICommunicationContract> 
        where TICommunicationContract : IControllerContract
    {
        public TICommunicationContract ClientController { get; init; }

        public ClientReference(string id, TICommunicationContract clientController) : base(id)
        {
            ClientController = clientController;
        }

        public ClientReference(string id, object? clientInfo, TICommunicationContract clientController) : base(id, clientInfo)
        {
            ClientController = clientController;
        }

        public ClientReference(ClientReference clientReference, TICommunicationContract clientController) : base(clientReference.Id, clientReference.ClientInfo)
        {
            ClientController = clientController;
        }
    }
}
