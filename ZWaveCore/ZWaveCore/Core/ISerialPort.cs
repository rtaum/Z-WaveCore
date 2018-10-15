using System.IO;

namespace ZWaveCore.Core
{
    public interface ISerialPort
    {
        Stream InputStream { get; }
        Stream OutputStream { get; }

        void Close();
        void Open();
    }
}
