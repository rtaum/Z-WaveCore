using System;
using System.Collections.Generic;
using ZWaveCore.Core;
using ZWaveCore.Core.Exceptions;
using ZWaveCore.Enums;

namespace ZWaveCore.Commands
{
    public class MeterSupportedReport : NodeReport
    {
        public readonly bool CanReset;
        public readonly MeterType Type;
        public readonly string[] Units;

        internal MeterSupportedReport(ZWaveNode node, byte[] payload) : base(node)
        {
            if (payload.Length < 2)
                throw new ReponseFormatException($"The response was not in the expected format. {GetType().Name}: Payload: {BitConverter.ToString(payload)}");

            CanReset = (payload[0] & 0x80) != 0;
            Type = (MeterType)Enum.ToObject(typeof(MeterType), payload[0] & 0x1F);

            var units = new List<string>();
            for (byte i = 0; i < 8; ++i)
            {
                if ((payload[1] & (1 << i)) == (1 << i))
                {
                    units.Add(MeterReport.GetUnit(Type, i));
                }
            }
            Units = units.ToArray();
        }

        public override string ToString()
        {
            return $"CanReset:{CanReset}, Type:{Type}, Units:[{string.Join(", ", Units)}]";
        }
    }
}
