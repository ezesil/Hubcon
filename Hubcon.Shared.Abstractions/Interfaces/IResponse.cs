﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hubcon.Shared.Abstractions.Interfaces
{
    public interface IResponse
    {
        public bool Success { get; }
        public string? Error { get; }
    }
}
