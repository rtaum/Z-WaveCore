using ZWaveCore.Enums;

namespace ZWaveCore.Messages
{
    class ControllerFunctionCompleted : ControllerFunctionMessage
    {
        public ControllerFunctionCompleted(Function function, byte[] payload)
            : base(MessageType.Response, function, payload)
        {
        }
    }
}
