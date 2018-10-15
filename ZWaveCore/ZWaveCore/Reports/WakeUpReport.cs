using ZWaveCore.Core;

namespace ZWaveCore.Reports
{
    public class WakeUpReport : NodeReport
    {
        public readonly bool Awake;

        internal WakeUpReport(ZWaveNode node) : base(node)
        {
            Awake = true;
        }

        public override string ToString()
        {
            return $"Awake:{Awake}";
        }
    }
}
