using System;
using System.Linq;
using System.Threading.Tasks;
using ZWaveCore.Commands;
using ZWaveCore.Core;

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

                var homeId = await controller.GetHomeID();
                //var nodes = await controller.GetNodes();
                //var multisensor = nodes.Skip(1).FirstOrDefault();
                //var supportedClasses = await multisensor.GetSupportedCommandClasses();
                //var command = multisensor.GetCommandClass<SensorMultiLevel>();
                //command.Changed += Command_Changed;
                //System.Threading.SpinWait.SpinUntil(() => false);


                //foreach (var node in nodes)
                //{
                //    var protocolInfo = await node.GetProtocolInfo();
                //    var supportedClasses = await node.GetSupportedCommandClasses();

                //    var command = node.GetCommandClass<SwitchBinary>();
                //    //var report = await command.Get();
                //    //await command.Set(true);
                //    var report = await command.Get();
                //    var tempreture = report.Value;
                //}
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

        private static void Command_Changed(object sender, ReportEventArgs<ZWaveCore.Reports.SensorMultiLevelReport> e)
        {
            throw new NotImplementedException();
        }
    }
}
