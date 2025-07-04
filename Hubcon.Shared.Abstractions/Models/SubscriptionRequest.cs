﻿using Hubcon.Shared.Abstractions.Interfaces;
using System.Text.Json;

namespace Hubcon.Shared.Abstractions.Models
{
    public class SubscriptionRequest : IOperationRequest
    {
        public string ContractName { get; }
        public string OperationName { get; }

        public Dictionary<string, object> Arguments { get; }

        public SubscriptionRequest(string operationName, string contractName, Dictionary<string, object>? args)
        {
            OperationName = operationName;
            ContractName = contractName;
            Arguments = args ?? [];
        }
    }
}
