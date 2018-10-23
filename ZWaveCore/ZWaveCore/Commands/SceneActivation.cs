using System;
using ZWaveCore.Core;
using ZWaveCore.Core.EventsArgs;
using ZWaveCore.Enums;
using ZWaveCore.Reports;

namespace ZWaveCore.Commands
{
    public class SceneActivation : CommandBase
    {
        public event EventHandler<ReportEventArgs<SceneActivationReport>> Changed;

        enum command : byte
        {
            Set = 0x01,
        }

        public SceneActivation(ZWaveNode node) : base(node, CommandClass.SceneActivation)
        {
        }

        protected internal override void HandleEvent(Command command)
        {
            base.HandleEvent(command);

            if (command.CommandID == Convert.ToByte(SceneActivation.command.Set))
            {
                var report = new SceneActivationReport(Node, command.Payload);
                OnChanged(new ReportEventArgs<SceneActivationReport>(report));
            }
        }

        protected virtual void OnChanged(ReportEventArgs<SceneActivationReport> e)
        {
            Changed?.Invoke(this, e);
        }
    }
}