﻿using System;
using ZWaveCore.Core;
using ZWaveCore.Core.Exceptions;

namespace ZWaveCore.Commands
{
    public class CentralSceneReport : NodeReport
    {
        public readonly byte Scene;

        internal CentralSceneReport(ZWaveNode node, byte[] payload) : base(node)
        {
            if (payload == null)
                throw new ArgumentNullException(nameof(payload));
            if (payload.Length < 3)
                throw new ReponseFormatException($"The response was not in the expected format. {GetType().Name}: Payload: {BitConverter.ToString(payload)}");

            Scene = payload[2];
        }

        public override string ToString()
        {
            return $"Scene:{Scene}";
        }
    }
}
