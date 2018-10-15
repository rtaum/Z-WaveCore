using System;
using System.Linq;
using ZWaveCore.Commands;
using ZWaveCore.Core.Exceptions;

namespace ZWaveCore.Core
{
    public class AssociationReport : NodeReport
    {
        public readonly byte GroupID;
        public readonly byte MaxNodesSupported;
        public readonly byte ReportsToFollow;
        public readonly byte[] Nodes;

        internal AssociationReport(ZWaveNode node, byte[] payload) : base(node)
        {
            if (payload == null)
                throw new ArgumentNullException(nameof(payload));

            if (payload.Length < 3)
                throw new ReponseFormatException($"The response was not in the expected format. {GetType().Name}: Payload: {BitConverter.ToString(payload)}");

            GroupID = payload[0];
            MaxNodesSupported = payload[1];
            ReportsToFollow = payload[2];
            Nodes = payload.Skip(3).ToArray();
        }

        public override string ToString()
        {
            return $"GroupID:{GroupID}, Nodes:{string.Join(", ", Nodes)}";
        }
    }
}