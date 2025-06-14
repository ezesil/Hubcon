﻿using Hubcon.Shared.Core.Websockets;
using Hubcon.Shared.Core.Websockets.Messages.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hubcon.Shared.Core.Websockets.Messages.Ping
{
    public record class PingMessage() : BaseMessage(MessageType.ping)
    {
        public Guid Id { get; } = Guid.NewGuid();
    }
}
