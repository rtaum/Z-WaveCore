using ZWaveCore.Enums;

namespace ZWaveCore.Core
{
    class NodeCommandCompleted : Message
    {
        public readonly byte CallbackID;
        public readonly TransmissionState TransmissionState;

        public NodeCommandCompleted(byte[] payload) :
            base(FrameHeader.StartOfFrame, MessageType.Request, Enums.Function.SendData)
        {
            CallbackID = payload[0];
            TransmissionState = (TransmissionState)payload[1];
        }

        public override string ToString()
        {
            return string.Concat(base.ToString(), " ", $"CallbackID:{CallbackID}, {TransmissionState}");
        }
    }
}
