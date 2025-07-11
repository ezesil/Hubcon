﻿using Hubcon.Server.Abstractions.Interfaces;
using Hubcon.Shared.Abstractions.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Hubcon.Server.Core.Pipelines.UpgradedPipeline
{
    public class OperationContext : IOperationContext
    {
        public string OperationName { get; init; }
        public IServiceProvider RequestServices { get; init; }
        public IOperationBlueprint Blueprint { get; init; }
        public ClaimsPrincipal? User { get; init; }
        public IDictionary<string, object> Items { get; } = new Dictionary<string, object>();
        public IOperationRequest Request { get; init; }
        public IOperationResult? Result { get; set; }
        public HttpContext? HttpContext { get; init; }
        public Exception? Exception { get; set; }
        public CancellationToken RequestAborted { get; init; }
    }
}
