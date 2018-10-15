using System;

namespace ZWaveCore.Core.Exceptions
{
    public class ChecksumException : CommunicationException
    {
        public ChecksumException() : base("Invalid checksum received.") { }
        public ChecksumException(string message) : base(message) { }
        public ChecksumException(string message, Exception inner) : base(message, inner) { }
    }
}
