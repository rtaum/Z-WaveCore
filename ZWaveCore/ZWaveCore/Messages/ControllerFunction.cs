using System;
using System.Collections.Generic;
using ZWaveCore.Enums;

namespace ZWaveCore.Messages
{
    class ControllerFunction : Message
    {
        public readonly byte[] Payload;

        public ControllerFunction(Function function, params byte[] payload)
            : base(FrameHeader.StartOfFrame, MessageType.Request, function)
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

        protected override List<byte> GetPayload()
        {
            var payload = base.GetPayload();
            if (Payload != null)
            {
                payload.AddRange(Payload);
            }
            return payload;
        }
    }
}
