using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ZWaveCore.Core
{
    public class NodeCollection : IEnumerable<ZWaveNode>
    {
        private readonly List<ZWaveNode> _nodes = new List<ZWaveNode>();

        internal void Add(ZWaveNode node)
        {
            _nodes.Add(node);
        }

        public ZWaveNode this[byte nodeID]
        {
            get { return _nodes.FirstOrDefault(element => element.NodeID == nodeID); }
        }

        public IEnumerator<ZWaveNode> GetEnumerator()
        {
            return _nodes.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _nodes.GetEnumerator();
        }
    }
}
