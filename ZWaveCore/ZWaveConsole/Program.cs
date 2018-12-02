using NLog;
using System;
using System.IO.Ports;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ZWaveCore.Commands;
using ZWaveCore.Core;
using ZWaveCore.Core.EventsArgs;
using ZWaveCore.Reports;

namespace ZWaveConsole
{
    class Program
    {
        private static ILogger Logger = LogManager.GetCurrentClassLogger();

        static void Main(string[] args)
        {
            Task.Run(async () => await CreateControllerAsync());
            Thread.Sleep(Timeout.Infinite);
        }

        private static async Task CreateControllerAsync()
        {
            var ports = SerialPort.GetPortNames();
            var portName = ports.First();

            var channel = new ZWaveChannel(portName);

            try
            {
                using (var controller = new ZWaveController(portName))
                {
                    try
                    {
                        controller.Open();
                        await ExploreNodes(controller);
                        Console.WriteLine($"Started at {DateTime.Now.ToShortTimeString()}");
                        Console.ReadLine();
                        Console.WriteLine("Waiting is over");
                    }
                    catch (Exception ecc)
                    {

                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                var m = ex.Message;
            }
        }

        private static async Task ExploreNodes(ZWaveController controller)
        {
            var allNodes = await controller.GetNodes();
            var nodes = allNodes.Skip(1).Take(1);
            foreach (var node in nodes)
            {
                var neighbours = await node.GetNeighbours();
                var protocolInfo = await node.GetProtocolInfo();
                var supportedClasses = await node.GetSupportedCommandClasses();
                var command = node.GetCommandClass<SensorMultiLevel>();
                command.Changed += Command_Changed;
            }
        }

        private static void Command_Changed(object sender, ReportEventArgs<SensorMultiLevelReport> e)
        {
            Console.WriteLine($"Measured value of {e.Report.Type} is {e.Report.Value}");
            Console.WriteLine($"Measured at {DateTime.Now.ToShortTimeString()}");
        }

        private static void Command1_Changed1(object sender, ReportEventArgs<SensorMultiLevelReport> e)
        {
            throw new NotImplementedException();
        }

        private static void Command1_Changed(object sender, ReportEventArgs<MultiChannelReport> e)
        {
            throw new NotImplementedException();
        }

        private static void Node_MessageReceived(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
