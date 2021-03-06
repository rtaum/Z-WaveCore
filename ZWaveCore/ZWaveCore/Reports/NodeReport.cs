﻿using System;
using ZWaveCore.Core;

namespace ZWaveCore.Reports
{
    public class NodeReport 
    {
        public readonly ZWaveNode Node;

        public NodeReport(ZWaveNode node)
        {
            if ((Node = node) == null)
                throw new ArgumentNullException(nameof(node));
        }
    }
}
