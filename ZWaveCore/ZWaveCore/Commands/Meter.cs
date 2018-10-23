using System;
using System.Threading;
using System.Threading.Tasks;
using ZWaveCore.Core;
using ZWaveCore.Core.Events;
using ZWaveCore.Enums;
using ZWaveCore.Extensions;
using ZWaveCore.Reports;

namespace ZWaveCore.Commands
{
    public class Meter : CommandBase
    {
        public event EventHandler<ReportEventArgs<MeterReport>> Changed;

        enum command
        {
            Get = 0x01,
            Report = 0x02,
            
            // Version 2
            SupportedGet = 0x03,
            SupportedReport = 0x04,
            Reset = 0x05
        }

        public Meter(ZWaveNode node) : base(node, CommandClass.Meter)
        {
        }

        public Task<MeterReport> Get()
        {
            return Get(CancellationToken.None);
        }

        public async Task<MeterReport> Get(CancellationToken cancellationToken)
        {
            ZWaveNode node = Node;
            var response = await Channel.Send(Node, new Command(Class, command.Get), command.Report, cancellationToken);
            return new MeterReport(Node, response);
        }

        public Task<MeterSupportedReport> GetSupported()
        {
            return GetSupported(CancellationToken.None);
        }

        public async Task<MeterSupportedReport> GetSupported(CancellationToken cancellationToken)
        {
            var response = await Channel.Send(Node, new Command(Class, command.SupportedGet), command.SupportedReport, cancellationToken);
            return new MeterSupportedReport(Node, response);
        }

        protected internal override void HandleEvent(Command command)
        {
            base.HandleEvent(command);

            if (command.CommandID == Convert.ToByte(Meter.command.Report))
            {
                var report = new MeterReport(Node, command.Payload);
                OnChanged(new ReportEventArgs<MeterReport>(report));
            }
        }

        protected virtual void OnChanged(ReportEventArgs<MeterReport> e)
        {
            var handler = Changed;
            handler?.Invoke(this, e);
        }
    }
}
