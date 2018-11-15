using System;
using System.IO;
using System.IO.Ports;
using ZWaveCore.Core.EventArguments;

namespace ZWaveCore.Core
{
    public class ZWaveSerialPort : ISerialPort
    {
        private readonly SerialPort _port;
        public event EventHandler<DataReceivedEventArgs> OnDataReceivedHandler;

        public Stream InputStream
        {
            get { return _port.BaseStream; }
        }

        public Stream OutputStream
        {
            get { return _port.BaseStream; }
        }

        public ZWaveSerialPort(string name)
        {
            _port = new SerialPort(name, 115200, Parity.None, 8, StopBits.One);
        }

        public void Open()
        {
            _port.Open();
            _port.DiscardInBuffer();
            _port.DiscardOutBuffer();
        }

        private void _port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            var etype = e.EventType;
            var port = sender as SerialPort;
            var data = new byte[port.BytesToRead];
            //port.Read(data, 0, port.BytesToRead);
            OnDataReceivedHandler?.Invoke(this, new DataReceivedEventArgs(data));
            var hashCode = port.GetHashCode();
        }

        public void Close()
        {
            //_port.DataReceived -= _port_DataReceived;
            _port.Close();
        }
    }
}
