using System;

namespace ZWaveCore.Core.Events
{
    public class ErrorEventArgs : EventArgs
    {
        public readonly Exception Error;

        public ErrorEventArgs(Exception error)
        {
            Error = error;
        }
    }
}
