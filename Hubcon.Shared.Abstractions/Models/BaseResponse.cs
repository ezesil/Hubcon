﻿using Hubcon.Shared.Abstractions.Interfaces;

namespace Hubcon.Shared.Abstractions.Models
{
    public abstract record class BaseResponse : IResponse
    {
        public abstract bool Success { get; }

        public abstract string? Error { get; }
    }
}
