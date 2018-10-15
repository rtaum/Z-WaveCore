using System;
using ZWaveCore.Commands;

namespace ZWaveCore.Core
{
    public class NodeEventArgs : EventArgs
    {
        public readonly byte NodeID;
        public readonly Command Command;

        public NodeEventArgs(byte nodeID, Command command)
        {
            if ((NodeID = nodeID) == 0)
                throw new ArgumentOutOfRangeException(nameof(NodeID), nodeID, "NodeID can not be 0");
            if ((Command = command) == null)
                throw new ArgumentNullException(nameof(command));
        }
    }
}
