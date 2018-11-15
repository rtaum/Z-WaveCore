using System;
using System.IO;
using ZWaveCore.Core.EventArguments;

namespace ZWaveCore.Core
{
    public interface ISerialPort
    {
        Stream InputStream { get; }

        Stream OutputStream { get; }

        event EventHandler<DataReceivedEventArgs> OnDataReceivedHandler;

        void Close();

        void Open();
    }
}
