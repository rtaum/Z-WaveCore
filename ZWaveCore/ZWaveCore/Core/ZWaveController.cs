using NLog;
using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ZWaveCore.Core.EventArguments;
using ZWaveCore.Core.EventsArgs;
using ZWaveCore.Enums;

namespace ZWaveCore.Core
{
    public class ZWaveController : IDisposable
    {
        private Task<ZWaveNodeCollection> _getNodes;
        private string _version;
        private uint? _homeID;
        private byte? _nodeID;
        public readonly ZWaveChannel Channel;
        public event EventHandler<ErrorEventArguments> Error;
        public event EventHandler ChannelClosed;

        private ZWaveController(ZWaveChannel channel)
        {
            Channel = channel;
        }

        public ZWaveController(ISerialPort port)
            : this(new ZWaveChannel(port))
        {
        }

        public ZWaveController(string portName)
            : this(new ZWaveChannel(portName))
        {
        }

        protected virtual void OnError(ErrorEventArguments e)
        {
            Error?.Invoke(this, e);
        }

        protected virtual void OnChannelClosed(EventArgs e)
        {
            ChannelClosed?.Invoke(this, e);
        }

        public void Open()
        {
            Channel.NodeEventReceived += Channel_NodeEventReceived;
            Channel.NodeUpdateReceived += Channel_NodeUpdateReceived;
            Channel.Error += Channel_Error;
            Channel.Closed += Channel_Closed;
            Channel.Open();
        }

        public Task<string> GetVersion()
        {
            return GetVersion(CancellationToken.None);
        }

        public async Task<string> GetVersion(CancellationToken cancellationToken)
        {
            if (_version == null)
            {
                var response = await Channel.Send(Function.GetVersion, cancellationToken);
                var data = response.TakeWhile(element => element != 0).ToArray();
                _version = Encoding.UTF8.GetString(data, 0, data.Length);
            }
            return _version;
        }

        public Task<uint> GetHomeID()
        {
            return GetHomeID(CancellationToken.None);
        }

        public async Task<uint> GetHomeID(CancellationToken cancellationToken)
        {
            if (_homeID == null)
            {
                var response = await Channel.Send(Function.MemoryGetId, cancellationToken);
                _homeID = PayloadConverter.ToUInt32(response);
            }
            return _homeID.Value;
        }

        public Task<byte> GetNodeID()
        {
            return GetNodeID(CancellationToken.None);
        }

        public async Task<byte> GetNodeID(CancellationToken cancellationToken)
        {
            if (_nodeID == null)
            {
                var response = await Channel.Send(Function.MemoryGetId, cancellationToken);
                _nodeID = response[4];
            }
            return _nodeID.Value;
        }

        public Task<ZWaveNodeCollection> DiscoverNodes()
        {
            return DiscoverNodes(CancellationToken.None);
        }

        public Task<ZWaveNodeCollection> DiscoverNodes(CancellationToken cancellationToken)
        {
            return _getNodes = Task.Run(async () =>
            {
                var response = await Channel.Send(Function.DiscoveryNodes, cancellationToken);
                var values = response.Skip(3).Take(29).ToArray();

                var nodes = new ZWaveNodeCollection();
                var bits = new BitArray(values);
                for (byte i = 0; i < bits.Length; i++)
                {
                    if (bits[i])
                    {
                        var node = new ZWaveNode((byte)(i + 1), this);
                        nodes.Add(node);
                    }
                }
                return nodes;
            });
        }

        public Task<ZWaveNodeCollection> GetNodes()
        {
            return GetNodes(CancellationToken.None);
        }

        public async Task<ZWaveNodeCollection> GetNodes(CancellationToken cancellationToken)
        {
            return await (_getNodes ?? (_getNodes = DiscoverNodes(cancellationToken)));
        }

        public void Dispose()
        {
            Close();
        }

        private void Channel_Error(object sender, ErrorEventArguments e)
        {
            OnError(e);
        }

        private void Channel_Closed(object sender, EventArgs e)
        {
            OnChannelClosed(e);
        }

        private async void Channel_NodeEventReceived(object sender, NodeEventArgs e)
        {
            try
            {
                var nodes = await GetNodes();
                var target = nodes[e.NodeID];
                if (target != null)
                {
                    target.HandleEvent(e.Command);
                }
            }
            catch (Exception ex)
            {
                OnError(new ErrorEventArguments(ex));
            }
        }

        private async void Channel_NodeUpdateReceived(object sender, NodeUpdateEventArgs e)
        {
            try
            {
                var nodes = await GetNodes();
                var target = nodes[e.NodeID];
                if (target != null)
                {
                    target.HandleUpdate();
                }
            }
            catch (Exception ex)
            {
                OnError(new ErrorEventArguments(ex));
            }
        }

        private void Close()
        {
            Channel.Error -= Channel_Error;
            Channel.NodeEventReceived -= Channel_NodeEventReceived;
            Channel.NodeUpdateReceived -= Channel_NodeUpdateReceived;
            Channel.Dispose();
        }
    }
}
