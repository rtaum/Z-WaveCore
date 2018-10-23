using System;
using System.Linq;
using System.Threading.Tasks;
using ZWaveCore.Commands;
using ZWaveCore.Core;
using ZWaveCore.Core.EventsArgs;
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
                var nodes = (await controller.GetNodes()).Skip(1).Take(1);
                foreach (var node in nodes)
                {
                    var protocolInfo = await node.GetProtocolInfo();
                    var supportedClasses = await node.GetSupportedCommandClasses();
                    var command = node.GetCommandClass<SensorMultiLevel>();
                    var report = await command.GetSupportedSensors();
                    var reports = report.SupportedSensorTypes;

                    var command1 = node.GetCommandClass<SensorBinary>();
                    var a = await command1.Get();
                    var command2 = node.GetCommandClass<ZWaveCore.Commands.Version>();
                    var v = await command2.Get();
                    var vv = command2.GetCommandClass(ZWaveCore.Enums.CommandClass.SwitchBinary);
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
