﻿using System;
using System.Linq;
using ZWaveCore.Core;
using ZWaveCore.Core.Exceptions;
using ZWaveCore.Enums;

namespace ZWaveCore.Reports
{
    public class MeterReport : NodeReport
    {
        public readonly MeterType Type;
        public readonly float Value;
        public readonly string Unit;
        public readonly byte Scale;

        internal MeterReport(ZWaveNode node, byte[] payload) : base(node)
        {
            if (payload == null)
                throw new ArgumentNullException(nameof(payload));
            if (payload.Length < 3)
                throw new ReponseFormatException($"The response was not in the expected format. {GetType().Name}: Payload: {BitConverter.ToString(payload)}");

            Type = (MeterType)(payload[0] & 0x1F);
            Value = PayloadConverter.ToFloat(payload.Skip(1).ToArray(), out Scale);
            Unit = GetUnit(Type, Scale);
        }

        public static string GetUnit(MeterType type, byte scale)
        {
            var electricityUnits = new[] { "kWh", "kVAh", "W", "pulses", "V", "A", "Power Factor", "" };
            var gasUnits = new[] { "cubic meters", "cubic feet", "", "pulses", "", "", "", "" };
            var waterUnits = new[] { "cubic meters", "cubic feet", "US gallons",  "pulses", "", "", "", ""};

            switch (type)
            {
                case MeterType.Electric: return electricityUnits[scale];
                case MeterType.Gas: return gasUnits[scale];
                case MeterType.Water: return waterUnits[scale];
                default: return string.Empty;
            }
        }

        public override string ToString()
        {
            return $"Type:{Type}, Value:\"{Value} {Unit}\"";
        }
    }
}
