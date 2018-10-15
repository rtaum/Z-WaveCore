using System;
using ZWaveCore.Core;
using ZWaveCore.Core.Exceptions;
using ZWaveCore.Reports;

namespace ZWaveCore.Commands
{
    public class AssociationGroupsReport : NodeReport
    {
        public readonly byte GroupsSupported;

        internal AssociationGroupsReport(ZWaveNode node, byte[] payload) : base(node)
        {
            if (payload == null)
                throw new ArgumentNullException(nameof(payload));
            if (payload.Length < 1)
                throw new ReponseFormatException($"The response was not in the expected format. {GetType().Name}: Payload: {BitConverter.ToString(payload)}");

            GroupsSupported = payload[0];
        }

        public override string ToString()
        {
            return $"GroupsSupported:{GroupsSupported}";
        }
    }
}