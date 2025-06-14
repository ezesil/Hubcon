﻿using Hubcon.Shared.Abstractions.Interfaces;
using System.Text.Json;

namespace Hubcon.Shared.Abstractions.Models
{
    public record class OperationRequest : IOperationRequest
    {
        public string ContractName { get; set; }
        public string OperationName { get; set; }
        public IEnumerable<JsonElement> Args { get; set; }

        public OperationRequest()
        {
            
        }

        public OperationRequest(string operationName, string contractName)
        {
            OperationName = operationName;
            ContractName = contractName;
            Args = new List<JsonElement>();
        }

        public OperationRequest(string methodName, string contractName, IEnumerable<JsonElement>? args)
        {
            OperationName = methodName;
            ContractName = contractName;
            Args = args ?? Array.Empty<JsonElement>();
        }
    }
}
