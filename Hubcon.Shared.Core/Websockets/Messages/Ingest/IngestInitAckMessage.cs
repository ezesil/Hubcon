﻿using Hubcon.Shared.Core.Websockets;
using Hubcon.Shared.Core.Websockets.Messages.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hubcon.Shared.Core.Websockets.Messages.Ingest
{
    public record class IngestInitAckMessage(string Id) : BaseMessage(MessageType.ingest_init_ack, Id);
}
