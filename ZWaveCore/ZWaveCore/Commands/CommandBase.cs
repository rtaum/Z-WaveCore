using ZWaveCore.Core;
using ZWaveCore.Enums;

namespace ZWaveCore.Commands
{
    public class CommandBase : ICommand
    {
        public ZWaveNode Node { get; private set; }

        public CommandClass Class { get; private set; }

        public CommandBase(ZWaveNode node, CommandClass commandClass)
        {
            Node = node;
            Class = commandClass;
        }

        public CommandBase()
        {

        }

        protected ZWaveChannel Channel
        {
            get { return Node.Controller.Channel; }
        }

        internal protected virtual void HandleEvent(Command command)
        {
        }
    }
}
