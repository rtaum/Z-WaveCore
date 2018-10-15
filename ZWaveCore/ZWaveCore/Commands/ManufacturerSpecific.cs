using System.Threading;
using System.Threading.Tasks;
using ZWaveCore.Core;
using ZWaveCore.Enums;
using ZWaveCore.Extensions;

namespace ZWaveCore.Commands
{
    public class ManufacturerSpecific : CommandBase
    {
        enum command
        {
            Get = 0x04,
            Report = 0x05
        }

        public ManufacturerSpecific(ZWaveNode node) : base(node, CommandClass.ManufacturerSpecific)
        {
        }

        public Task<ManufacturerSpecificReport> Get()
        {
            return Get(CancellationToken.None);
        }

        public async Task<ManufacturerSpecificReport> Get(CancellationToken cancellationToken)
        {
            var response = await Channel.Send(Node, new Command(Class, command.Get), command.Report, cancellationToken);
            return new ManufacturerSpecificReport(Node, response);
        }
    }
}
