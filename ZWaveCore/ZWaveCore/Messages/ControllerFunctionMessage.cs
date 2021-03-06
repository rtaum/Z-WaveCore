﻿using System;
using ZWaveCore.Enums;

namespace ZWaveCore.Messages
{
    class ControllerFunctionMessage : Message
    {
        public readonly byte[] Payload;

        public ControllerFunctionMessage(MessageType type, Function function, byte[] payload)
            : base(FrameHeader.StartOfFrame, type, function)
        {
            Payload = payload;
        }

        public override string ToString()
        {
            if (Payload != null)
            {
                return string.Concat(base.ToString(), " ", $"Payload:{BitConverter.ToString(Payload)}");
            }
            return base.ToString();
        }
    }
}
