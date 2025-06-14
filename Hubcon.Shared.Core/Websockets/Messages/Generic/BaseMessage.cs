﻿namespace Hubcon.Shared.Core.Websockets.Messages.Generic
{
    public record class BaseMessage
    {
        public MessageType Type { get; set; } = default!;

        public BaseMessage()
        {
            
        }

        protected BaseMessage(MessageType type)
        {
            Type = type;
        }
    }
}