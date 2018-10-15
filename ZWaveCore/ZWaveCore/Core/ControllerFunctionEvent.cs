using ZWaveCore.Enums;

namespace ZWaveCore.Core
{
    class ControllerFunctionEvent : ControllerFunctionMessage
    {
        public ControllerFunctionEvent(Function function, byte[] payload)
            : base(MessageType.Request, function, payload)
        {
        }
    }
}
