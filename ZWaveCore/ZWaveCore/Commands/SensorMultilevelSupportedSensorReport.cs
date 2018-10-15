using System.Collections;
using System.Collections.Generic;
using ZWaveCore.Core;
using ZWaveCore.Enums;
using ZWaveCore.Reports;

namespace ZWaveCore.Commands
{
    public class SensorMultilevelSupportedSensorReport : NodeReport
    {
        public IReadOnlyCollection<SensorType> SupportedSensorTypes { get; private set; }

        public SensorMultilevelSupportedSensorReport(ZWaveNode node, byte[] payload)
            : base(node)
        {
            LinkedList<SensorType> supportedTypes = new LinkedList<SensorType>();
            BitArray bits = new BitArray(payload);
            for (byte i = 0; i < bits.Length; i++)
            {
                if (bits[i])
                {
                    supportedTypes.AddLast((SensorType)(i + 1));
                }
            }

            SupportedSensorTypes = (IReadOnlyCollection<SensorType>)supportedTypes;
        }
    }
}
