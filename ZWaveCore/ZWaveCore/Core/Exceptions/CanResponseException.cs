using System;

namespace ZWaveCore.Core.Exceptions
{
    public class CanResponseException : ResponseException
    {
        public CanResponseException() : base("CAN response received.") { }
        public CanResponseException(string message) : base(message) { }
        public CanResponseException(string message, Exception inner) : base(message, inner) { }
    }
}
