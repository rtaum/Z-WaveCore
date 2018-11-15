using System;
using System.Threading;
using System.Threading.Tasks;
using ZWaveCore.Core;
using ZWaveCore.Enums;
using ZWaveCore.Extensions;
using ZWaveCore.Reports;

namespace ZWaveCore.Commands
{
    public class ZWaveVersion : CommandBase
    {
        enum command : byte
        {
            Get = 0x11,
            Report = 0x12,
            CommandClassGet = 0x13,
            CommandClassReport = 0x14
        }


        public ZWaveVersion(ZWaveNode node) :
            base(node, CommandClass.Version)
        {
        }

        public Task<VersionReport> Get()
        {
            return Get(CancellationToken.None);
        }

        public async Task<VersionReport> Get(CancellationToken cancellationToken)
        {
            var response = await Channel.Send(Node, new Command(Class, command.Get), command.Report, cancellationToken);
            return new VersionReport(Node, response);
        }

        public Task<VersionCommandClassReport> GetCommandClass(CommandClass @class)
        {
            return GetCommandClass(@class, CancellationToken.None);
        }

        public async Task<VersionCommandClassReport> GetCommandClass(CommandClass @class, CancellationToken cancellationToken)
        {
            var response = await Channel.Send(Node,
                new Command(Class, command.CommandClassGet, Convert.ToByte(@class)),
                command.CommandClassReport,
                VersionCommandClassReport.GetResponseValidatorForCommandClass(Node, @class),
                cancellationToken);
            return new VersionCommandClassReport(Node, response);
        }
    }
}
