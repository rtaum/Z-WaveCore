using System;
using System.Threading;
using System.Threading.Tasks;
using ZWaveCore.Core;
using ZWaveCore.Core.Events;
using ZWaveCore.Enums;
using ZWaveCore.Reports;

namespace ZWaveCore.Commands
{
    public class SwitchBinary : EndpointSupportedCommandClassBase
    {
        public event EventHandler<ReportEventArgs<SwitchBinaryReport>> Changed;

        public enum command
        {
            Set = 0x01,
            Get = 0x02,
            Report = 0x03
        }

        public SwitchBinary(ZWaveNode node)
            : base(node, CommandClass.SwitchBinary)
        { }

        internal SwitchBinary(ZWaveNode node, byte endpointId)
            : base(node, CommandClass.SwitchBinary, endpointId)
        { }

        public Task<SwitchBinaryReport> Get()
        {
            return Get(CancellationToken.None);
        }

        public async Task<SwitchBinaryReport> Get(CancellationToken cancellationToken)
        {
            var response = await Send(new Command(Class, command.Get), command.Report, cancellationToken);
            return new SwitchBinaryReport(Node, response);
        }

        public Task Set(bool value)
        {
            return Set(value, CancellationToken.None);
        }

        public async Task Set(bool value, CancellationToken cancellationToken)
        {
            await Send(new Command(Class, command.Set, value ? (byte)0xFF : (byte)0x00), cancellationToken);
        }

        protected internal override void HandleEvent(Command command)
        {
            base.HandleEvent(command);

            var report = new SwitchBinaryReport(Node, command.Payload);
            OnChanged(new ReportEventArgs<SwitchBinaryReport>(report));
        }

        protected virtual void OnChanged(ReportEventArgs<SwitchBinaryReport> e)
        {
            var handler = Changed;
            handler?.Invoke(this, e);
        }

    }
}
