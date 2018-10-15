using ZWaveCore.Core;
using ZWaveCore.Enums;

namespace ZWaveCore.Commands
{
    public interface ICommand
    {
        ZWaveNode Node { get; }

        CommandClass Class { get; }
    }
}
