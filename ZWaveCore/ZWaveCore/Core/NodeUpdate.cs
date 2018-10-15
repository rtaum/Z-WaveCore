using System;
using ZWaveCore.Core.Exceptions;
using ZWaveCore.Enums;

namespace ZWaveCore.Core
{
    class NodeUpdate : Message
    {
        public readonly NodeUpdateState UpdateState;
        public readonly byte NodeID;

        public NodeUpdate(byte[] payload)
            : base(FrameHeader.StartOfFrame, MessageType.Request, Enums.Function.ApplicationUpdate)
        {
            if (payload == null)
                throw new ArgumentNullException(nameof(payload));
            if (payload.Length < 2)
                throw new ReponseFormatException($"The response was not in the expected format. NodeEvent, payload: {BitConverter.ToString(payload)}");

            UpdateState = (NodeUpdateState)payload[0];
            NodeID = payload[1];
        }
    }
}
