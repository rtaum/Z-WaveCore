using System;

namespace ZWaveCore.Core.Exceptions
{
    public class CancelledResponseException : ResponseException
    {
        public CancelledResponseException() : base("CAN response received.") { }
        public CancelledResponseException(string message) : base(message) { }
        public CancelledResponseException(string message, Exception inner) : base(message, inner) { }
    }
}
