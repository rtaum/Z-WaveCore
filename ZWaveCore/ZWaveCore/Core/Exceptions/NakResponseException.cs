using System;

namespace ZWaveCore.Core.Exceptions
{
    public class NakResponseException : ResponseException
    {
        public NakResponseException() : base("NAK response received.") { }
        public NakResponseException(string message) : base(message) { }
        public NakResponseException(string message, Exception inner) : base(message, inner) { }
    }
}
