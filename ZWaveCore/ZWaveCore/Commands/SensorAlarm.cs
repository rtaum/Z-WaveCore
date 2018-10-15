﻿using System;
using System.Threading;
using System.Threading.Tasks;
using ZWaveCore.Core;
using ZWaveCore.Enums;
using ZWaveCore.Extensions;
using ZWaveCore.Reports;

namespace ZWaveCore.Commands
{
    public class SensorAlarm : CommandBase
    {
        public event EventHandler<ReportEventArgs<SensorAlarmReport>> Changed;

        enum command
        {
            Get = 0x01,
            Report = 0x02,
            SupportedGet = 0x03,
            SupportedReport = 0x04
        }

        public SensorAlarm(ZWaveNode node) : base(node, CommandClass.SensorAlarm)
        {
        }

        public Task<SensorAlarmReport> Get(AlarmType type)
        {
            return Get(type, CancellationToken.None);
        }

        public async Task<SensorAlarmReport> Get(AlarmType type, CancellationToken cancellationToken)
        {
            var response = await Channel.Send(Node, new Command(Class, command.Get, Convert.ToByte(type)), command.Report, cancellationToken);
            return new SensorAlarmReport(Node, response);
        }

        protected internal override void HandleEvent(Command command)
        {
            base.HandleEvent(command);

            var report = new SensorAlarmReport(Node, command.Payload);
            OnChanged(new ReportEventArgs<SensorAlarmReport>(report));
        }

        protected virtual void OnChanged(ReportEventArgs<SensorAlarmReport> e)
        {
            var handler = Changed;
            handler?.Invoke(this, e);
        }

    }
}