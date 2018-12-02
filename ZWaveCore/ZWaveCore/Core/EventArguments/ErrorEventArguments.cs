using System;

namespace ZWaveCore.Core.EventArguments
{
    public class ErrorEventArguments : EventArgs
    {
        public readonly Exception Error;

        public ErrorEventArguments(Exception error)
        {
            Error = error;
        }
    }
}
