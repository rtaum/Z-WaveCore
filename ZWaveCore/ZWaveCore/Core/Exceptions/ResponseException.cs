using System;

namespace ZWaveCore.Core.Exceptions
{
    public class ResponseException : CommunicationException
    {
        public ResponseException() : base("Invalid response received.") { }
        public ResponseException(string message) : base(message) { }
        public ResponseException(string message, Exception inner) : base(message, inner) { }
    }
}
