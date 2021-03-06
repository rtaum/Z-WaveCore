﻿using System;
using ZWaveCore.Core;
using ZWaveCore.Core.Exceptions;

namespace ZWaveCore.Reports
{
    public class BatteryReport : NodeReport
    {
        public readonly byte Value;
        public readonly bool IsLow;

        internal BatteryReport(ZWaveNode node, byte[] payload) : base(node)
        {
            if (payload == null)
                throw new ArgumentNullException(nameof(payload));
            if (payload.Length < 1)
                throw new ReponseFormatException($"The response was not in the expected format. {GetType().Name}: Payload: {BitConverter.ToString(payload)}");

            IsLow = payload[0] == 0xFF;
            Value = IsLow ? (byte)0x00 : payload[0];
        }

        public override string ToString()
        {
            return IsLow ? "Low" : $"Value:{Value}%";
        }
    }
}
