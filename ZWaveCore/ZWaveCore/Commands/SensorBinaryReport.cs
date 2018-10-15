using System;
using ZWaveCore.Core;
using ZWaveCore.Core.Exceptions;

namespace ZWaveCore.Commands
{
    public class SensorBinaryReport : NodeReport
    {
        public readonly bool Value;

        internal SensorBinaryReport(ZWaveNode node, byte[] payload) : base(node)
        {
            if (payload == null)
                throw new ArgumentNullException(nameof(payload));
            if (payload.Length < 1)
                throw new ReponseFormatException($"The response was not in the expected format. {GetType().Name}: Payload: {BitConverter.ToString(payload)}");

            Value = payload[0] == 0xFF;
        }

        public override string ToString()
        {
            return $"Value:{Value}";
        }
    }
}
