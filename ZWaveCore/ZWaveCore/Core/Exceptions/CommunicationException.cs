using System;

namespace ZWaveCore.Core.Exceptions
{
    public class CommunicationException : Exception
    {
        public CommunicationException() : base("Communication error") { }
        public CommunicationException(string message) : base(message) { }
        public CommunicationException(string message, Exception inner) : base(message, inner) { }
    }
}
