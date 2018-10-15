using System;

namespace ZWaveCore.Core.Exceptions
{
    public class UnknownFrameException : CommunicationException
    {
        public UnknownFrameException() : base("Unknown frame received.") { }
        public UnknownFrameException(string message) : base(message) { }
        public UnknownFrameException(string message, Exception inner) : base(message, inner) { }
    }
}
