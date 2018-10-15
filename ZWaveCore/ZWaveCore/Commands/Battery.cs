using System;
using System.Threading;
using System.Threading.Tasks;
using ZWaveCore.Core;
using ZWaveCore.Enums;
using ZWaveCore.Extensions;
using ZWaveCore.Reports;

namespace ZWaveCore.Commands
{
    public class Battery : CommandBase
    {
        enum command
        {
            Get = 0x02,
            Report = 0x03
        }

        public event EventHandler<ReportEventArgs<BatteryReport>> Changed;

        public Battery(ZWaveNode node) : base(node, CommandClass.Battery)
        {
        }

        public Task<BatteryReport> Get()
        {
            return Get(CancellationToken.None);
        }

        public async Task<BatteryReport> Get(CancellationToken cancellationToken)
        {
            var response = await Channel.Send(Node, new Command(Class, command.Get), command.Report, cancellationToken);
            return new BatteryReport(Node, response);
        }

        protected internal override void HandleEvent(Command command)
        {
            base.HandleEvent(command);

            var report = new BatteryReport(Node, command.Payload);
            OnChanged(new ReportEventArgs<BatteryReport>(report));
        }

        protected virtual void OnChanged(ReportEventArgs<BatteryReport> e)
        {
            var handler = Changed;
            handler?.Invoke(this, e);
        }
    }
}
