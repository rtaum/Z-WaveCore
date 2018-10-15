using ZWaveCore.Enums;

namespace ZWaveCore.Core
{
    class ControllerFunctionCompleted : ControllerFunctionMessage
    {
        public ControllerFunctionCompleted(Function function, byte[] payload)
            : base(MessageType.Response, function, payload)
        {
        }
    }
}
