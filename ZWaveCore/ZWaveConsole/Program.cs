using System;
using System.Linq;
using System.Threading.Tasks;
using ZWaveCore.Commands;
using ZWaveCore.Core;
using ZWaveCore.Core.Events;
using ZWaveCore.Reports;

namespace ZWaveConsole
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var ports = System.IO.Ports.SerialPort.GetPortNames();
            var portName = ports.First();

            var channel = new ZWaveChannel(portName);
            var controller = new ZWaveController(portName);
            try
            {
                controller.Open();
                var nodes = await controller.GetNodes();
                foreach (var node in nodes)
                {
                    var protocolInfo = await node.GetProtocolInfo();
                    var supportedClasses = await node.GetSupportedCommandClasses();

                    var command = node.GetCommandClass<SwitchBinary>();
                    var report = await command.Get();
                    await command.Set(true);
                }
            }
            catch(Exception ex)
            {
                var m = ex.Message;
            }
            finally
            {
                controller.Close();
            }

        }

        private static void Command_Changed(object sender, ReportEventArgs<SensorMultiLevelReport> e)
        {
            throw new NotImplementedException();
        }
    }
}
