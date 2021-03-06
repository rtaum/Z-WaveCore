﻿using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ZWaveCore.Core;
using ZWaveCore.Enums;
using ZWaveCore.Extensions;
using ZWaveCore.Reports;

namespace ZWaveCore.Commands
{
    public class Color : CommandBase
    {
        enum command
        {
            Get = 0x03,
            Report = 0x04,
            Set = 0x05,
        }

        public Color(ZWaveNode node) : base(node, CommandClass.Color)
        {
        }

        public Task Set(ColorComponent[] components)
        {
            return Set(components, CancellationToken.None);
        }

        public async Task Set(ColorComponent[] components, CancellationToken cancellationToken)
        {
            var payload = new List<byte>();
            payload.Add((byte)components.Length);
            payload.AddRange(components.SelectMany(element => element.ToBytes()));
            await Channel.Send(Node, new Command(Class, command.Set, payload.ToArray()), cancellationToken);
        }

        public Task<ColorReport> Get(byte componentID)
        {
            return Get(componentID, CancellationToken.None);
        }

        public async Task<ColorReport> Get(byte componentID, CancellationToken cancellationToken)
        {
            var response = await Channel.Send(Node, new Command(Class, command.Get, componentID), command.Report, cancellationToken);
            return new ColorReport(Node, response);
        }
    }
}
