using System;

namespace ZWaveCore.Core.Exceptions
{
    public class NegativeAcknowledgeException : ResponseException
    {
        public NegativeAcknowledgeException() : base("NAK response received.") { }
        public NegativeAcknowledgeException(string message) : base(message) { }
        public NegativeAcknowledgeException(string message, Exception inner) : base(message, inner) { }
    }
}
