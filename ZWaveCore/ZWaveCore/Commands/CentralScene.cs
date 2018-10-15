using System;
using ZWaveCore.Core;
using ZWaveCore.Enums;

namespace ZWaveCore.Commands
{
    public class CentralScene : CommandBase
    {
        public event EventHandler<ReportEventArgs<CentralSceneReport>> Changed;

        public CentralScene(ZWaveNode node) : base(node, CommandClass.CentralScene)
        {
        }

        protected internal override void HandleEvent(Command command)
        {
            base.HandleEvent(command);

            var report = new CentralSceneReport(Node, command.Payload);
            OnChanged(new ReportEventArgs<CentralSceneReport>(report));
        }

        protected virtual void OnChanged(ReportEventArgs<CentralSceneReport> e)
        {
            var handler = Changed;
            if (handler != null)
            {
                handler(this, e);
            }
        }
    }
}
