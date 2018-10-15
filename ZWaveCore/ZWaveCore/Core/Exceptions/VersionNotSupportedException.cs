using System;

namespace ZWaveCore.Core.Exceptions
{
    public class VersionNotSupportedException : Exception
    {
        public VersionNotSupportedException() : base("version not supported") { }
        public VersionNotSupportedException(string message) : base(message) { }
        public VersionNotSupportedException(string message, Exception inner) : base(message, inner) { }
    }
}
