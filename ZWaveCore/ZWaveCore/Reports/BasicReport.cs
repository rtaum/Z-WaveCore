using System;
using ZWaveCore.Core;
using ZWaveCore.Core.Exceptions;

namespace ZWaveCore.Reports
{
    public class BasicReport : NodeReport
    {
        public readonly byte Value;

        internal BasicReport(ZWaveNode node, byte[] payload) : base(node)
        {
            if (payload == null)
                throw new ArgumentNullException(nameof(payload));
            if (payload.Length < 1)
                throw new ReponseFormatException($"The response was not in the expected format. {GetType().Name}: Payload: {BitConverter.ToString(payload)}");

            Value = payload[0];
        }

        public override string ToString()
        {
            return $"Value:{Value}";
        }
    }
}
