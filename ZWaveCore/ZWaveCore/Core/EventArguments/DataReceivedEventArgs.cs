namespace ZWaveCore.Core.EventArguments
{
    public class DataReceivedEventArgs
    {
        public byte[] Data { get; private set; }

        public DataReceivedEventArgs(byte[] data)
        {
            Data = data;
        }
    }
}
