﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hubcon.Shared.Core.Websockets.Interfaces
{
    public interface IAwaitableAckMessage
    {
        Task<bool> WaitAckAsync();
    }
}