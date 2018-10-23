using System;

namespace ZWaveCore.Core.EventArguments
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
