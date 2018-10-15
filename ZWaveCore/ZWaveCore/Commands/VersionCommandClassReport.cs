using System;
using ZWaveCore.Core;
using ZWaveCore.Core.Exceptions;
using ZWaveCore.Enums;

namespace ZWaveCore.Commands
{
    public class VersionCommandClassReport : NodeReport
    {
        public readonly CommandClass Class;
        public readonly byte Version;

        internal VersionCommandClassReport(ZWaveNode node, byte[] payload) : base(node)
        {
            if (payload == null)
                throw new ArgumentNullException(nameof(payload));
            if (payload.Length < 2)
                throw new ReponseFormatException($"The response was not in the expected format. {GetType().Name}: Payload: {BitConverter.ToString(payload)}");

            Class = (CommandClass)Enum.ToObject(typeof(CommandClass), payload[0]);
            Version = payload[1];
        }

        internal static Func<byte[], bool> GetResponseValidatorForCommandClass(ZWaveNode node, CommandClass @class)
        {
            return payload =>
            {
                try
                {
                    var report = new VersionCommandClassReport(node, payload);
                    return report.Class == @class;
                }
                catch
                {
                    return false;
                }
            };
        }

        public override string ToString()
        {
            return $"Class:{Class}, Version:{Version}";
        }
    }
}
