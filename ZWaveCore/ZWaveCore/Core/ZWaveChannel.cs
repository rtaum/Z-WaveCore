using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ZWaveCore.Core.EventArguments;
using ZWaveCore.Core.EventsArgs;
using ZWaveCore.Core.Exceptions;
using ZWaveCore.Enums;
using ZWaveCore.Messages;

using ErrorEventArgs = ZWaveCore.Core.EventArguments.ErrorEventArgs;

namespace ZWaveCore.Core
{
    public class ZWaveChannel : IDisposable
    {
        private readonly SemaphoreSlim _semaphore;
        private Task _portReadTask;
        private Task _processEventsTask;
        private Task _processUpdatesTask;
        private Task _transmitTask;
        private BlockingCollection<NodeEvent> _eventQueue;
        private BlockingCollection<NodeUpdate> _updateQueue;
        private BlockingCollection<Message> _transmitQueue;
        private BlockingCollection<Message> _responseQueue;

        public readonly ISerialPort Port;
        public TextWriter Log { get; set; }
        public TimeSpan ReceiveTimeout = TimeSpan.FromSeconds(2);
        public TimeSpan ResponseTimeout = TimeSpan.FromSeconds(5);
        public event EventHandler<NodeEventArgs> NodeEventReceived;
        public event EventHandler<NodeUpdateEventArgs> NodeUpdateReceived;
        public event EventHandler<ErrorEventArgs> Error;
        public event EventHandler Closed;

        public int MaxRetryCount { get; set; } = 3;

        public ZWaveChannel(ISerialPort port)
        {
            if ((Port = port) == null)
                throw new ArgumentNullException(nameof(port));

            _semaphore = new SemaphoreSlim(1, 1);
            //port.OnDataReceivedHandler += PortDataReceived;
        }

        //private async void PortDataReceived(object sender, DataReceivedEventArgs e)
        //{
        //    var port = sender as ISerialPort;
        //    if (port == null)
        //    {
        //        throw new ArgumentException(nameof(sender));
        //    }

        //    try
        //    {
        //        // wait for message received (blocking)
        //        var message = await Message.Read(port.InputStream).ConfigureAwait(false);
        //        LogMessage($"Received: {message}");

        //        // ignore ACK, no processing of ACK needed
        //        if (message == Message.Acknowledge)
        //            return;

        //        // is it a eventmessage from a node?
        //        if (message is NodeEvent)
        //        {
        //            // yes, so add to eventqueue
        //            _eventQueue.Add(message);
        //            // send ACK to controller
        //            _transmitQueue.Add(Message.Acknowledge);
        //            // we're done
        //            return;
        //        }

        //        // is it a updatemessage from a node?
        //        if (message is NodeUpdate)
        //        {
        //            // yes, so add to eventqueue
        //            _eventQueue.Add(message);
        //            // send ACK to controller
        //            _transmitQueue.Add(Message.Acknowledge);
        //            // we're done
        //            return;
        //        }

        //        // not a event, so it's a response to a request
        //        _responseQueue.Add(message);
        //        // send ACK to controller
        //        _transmitQueue.Add(Message.Acknowledge);
        //    }
        //    catch (ChecksumException ex)
        //    {
        //        LogMessage($"Exception: {ex}");
        //        _transmitQueue.Add(Message.NegativeAcknowledge);
        //    }
        //    catch (UnknownFrameException ex)
        //    {
        //        // probably out of sync on the serial port
        //        // ToDo: handle gracefully 
        //        OnError(new ErrorEventArgs(ex));
        //    }
        //    catch (IOException)
        //    {
        //        // port closed, we're done so return
        //        OnClosed(EventArgs.Empty);
        //        return;
        //    }
        //    catch (Exception ex)
        //    {
        //        // just raise error event. don't break reading of serial port
        //        OnError(new ErrorEventArgs(ex));
        //    }
        //}

        public ZWaveChannel(string portName)
             : this(new ZWaveSerialPort(portName))
        {
        }

        public void Open()
        {
            Port.Open();

            // create tasks, on open or re-open
            _eventQueue = new BlockingCollection<NodeEvent>();
            _updateQueue = new BlockingCollection<NodeUpdate>();
            _transmitQueue = new BlockingCollection<Message>();
            _responseQueue = new BlockingCollection<Message>();

            _processEventsTask = new Task(() => ProcessQueue(_eventQueue, OnNodeEventReceived),
                TaskCreationOptions.LongRunning);
            _processUpdatesTask = new Task(() => ProcessQueue(_updateQueue, OnNodeUpdateReceived),
                TaskCreationOptions.LongRunning);
            _transmitTask = new Task(() => ProcessQueue(_transmitQueue, OnTransmit),
                TaskCreationOptions.LongRunning);
            _portReadTask = new Task(() => ReadPort(Port),
                TaskCreationOptions.LongRunning);

            // start tasks
            _portReadTask.Start();
            _processEventsTask.Start();
            _transmitTask.Start();
        }

        public void Dispose()
        {
            Close();
        }

        private void Close()
        {
            Port.Close();

            _eventQueue.CompleteAdding();
            _updateQueue.CompleteAdding();
            _responseQueue.CompleteAdding();
            _transmitQueue.CompleteAdding();

            Task.WaitAll(new []
            {
                _portReadTask,
                _processEventsTask,
                _processUpdatesTask,
                _transmitTask
            });
        }

        public Task<byte[]> Send(Function function, params byte[] payload)
        {
            return Send(function, CancellationToken.None, payload);
        }

        public Task<byte[]> Send(Function function, CancellationToken cancellationToken, params byte[] payload)
        {
            return Send(function, payload, (message) =>
            {
                var controllerFunctionCompleted = message as ControllerFunctionCompleted;
                return controllerFunctionCompleted?.Function == function;
            }, cancellationToken);
        }

        public Task<byte[]> Send(Function function, byte[] payload, Func<byte[], bool> predicate)
        {
            return Send(function, payload, predicate, CancellationToken.None);
        }

        public Task<byte[]> Send(Function function, byte[] payload, Func<byte[], bool> predicate, CancellationToken cancellationToken)
        {
            return Send(function, payload, (message) =>
            {
                var controllerFunctionCompleted = message as ControllerFunctionCompleted;
                return controllerFunctionCompleted != null && predicate(controllerFunctionCompleted.Payload);
            }, cancellationToken);
        }

        public Task Send(byte nodeID, Command command)
        {
            return Send(nodeID, command, CancellationToken.None);
        }

        public Task Send(byte nodeID, Command command, CancellationToken cancellationToken)
        {
            if (nodeID == 0)
                throw new ArgumentOutOfRangeException(nameof(nodeID), nodeID, "nodeID can not be 0");
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            return Exchange(async () =>
            {
                var request = new NodeCommand(nodeID, command);
                _transmitQueue.Add(request);

                await WaitForResponse((message) =>
                {
                    var nodeCommand = message as NodeCommandCompleted;
                    return nodeCommand?.CallbackID == request.CallbackID;
                }, cancellationToken).ConfigureAwait(false);

                return null;
            }, $"NodeID:{nodeID:D3}, Command:{command}", cancellationToken);
        }

        public Task<byte[]> Send(byte nodeID, Command command, byte responseCommandID)
        {
            return Send(nodeID, command, responseCommandID, CancellationToken.None);
        }

        public Task<byte[]> Send(byte nodeID, Command command, byte responseCommandID, CancellationToken cancellationToken)
        {
            return Send(nodeID, command, responseCommandID, null, cancellationToken);
        }

        public Task<byte[]> Send(byte nodeID, Command command, byte responseCommandID, Func<byte[], bool> payloadValidation)
        {
            return Send(nodeID, command, responseCommandID, payloadValidation, CancellationToken.None);
        }

        public Task<byte[]> Send(byte nodeID, Command command, byte responseCommandID, Func<byte[], bool> payloadValidation, CancellationToken cancellationToken)
        {
            if (nodeID == 0)
                throw new ArgumentOutOfRangeException(nameof(nodeID), nodeID, "nodeID can not be 0");
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            return Exchange(async () =>
            {
                var completionSource = new TaskCompletionSource<Command>();

                EventHandler<NodeEventArgs> onNodeEventReceived = (_, e) =>
                {
                    if (e.NodeID == nodeID &&
                        e.Command.ClassID == command.ClassID &&
                        e.Command.CommandID == responseCommandID)
                    {
                        if (payloadValidation == null || payloadValidation(e.Command.Payload))
                        {
                            // BugFix: 
                            // http://stackoverflow.com/questions/19481964/calling-taskcompletionsource-setresult-in-a-non-blocking-manner
                            Task.Run(() => completionSource.TrySetResult(e.Command));
                        }
                    }
                };

                var request = new NodeCommand(nodeID, command);
                _transmitQueue.Add(request);

                NodeEventReceived += onNodeEventReceived;
                try
                {
                    await WaitForResponse((message) =>
                    {
                        var nodeCommandCompleted = message as NodeCommandCompleted;
                        return nodeCommandCompleted?.CallbackID == request.CallbackID;
                    }, cancellationToken).ConfigureAwait(false);

                    try
                    {
                        using (var cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken))
                        {
                            cancellationTokenSource.CancelAfter(ResponseTimeout);
                            cancellationTokenSource.Token.Register(() => completionSource.TrySetCanceled(), useSynchronizationContext: false);

                            var response = await completionSource.Task.ConfigureAwait(false);
                            return response.Payload;
                        }
                    }
                    catch (TaskCanceledException)
                    {
                        // Rethrow only if the external cancellation token was canceled.
                        //
                        if (cancellationToken.IsCancellationRequested)
                        {
                            throw;
                        }

                        throw new TimeoutException();
                    }
                }
                finally
                {
                    NodeEventReceived -= onNodeEventReceived;
                }
            }, $"NodeID:{nodeID:D3}, Command:[{command}], Reponse:{responseCommandID}", cancellationToken);
        }

        private Task<byte[]> Send(Function function, byte[] payload, Func<ControllerFunctionMessage, bool> predicate)
        {
            return Send(function, payload, predicate, CancellationToken.None);
        }

        private Task<byte[]> Send(Function function,
            byte[] payload,
            Func<ControllerFunctionMessage, bool> predicate,
            CancellationToken cancellationToken)
        {
            return Exchange(async () =>
            {
                var request = new ControllerFunction(function, payload);
                _transmitQueue.Add(request);

                var response = await WaitForResponse(
                    (message) => predicate((ControllerFunctionMessage)message),
                    cancellationToken).ConfigureAwait(false);

                return ((ControllerFunctionMessage)response).Payload;
            }, $"{function} {(payload != null ? BitConverter.ToString(payload) : string.Empty)}", cancellationToken);
        }

        private async Task<Message> WaitForResponse(Func<Message, bool> predicate, CancellationToken cancellationToken)
        {
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            while (!cancellationToken.IsCancellationRequested)
            {
                var result = await Task.Run(() =>
                {
                    var message = default(Message);
                    _responseQueue.TryTake(out message, (int)ResponseTimeout.TotalMilliseconds, cancellationToken);
                    return message;
                }).ConfigureAwait(false);

                if (result == null)
                    throw new TimeoutException();
                if (result == Message.NegativeAcknowledge)
                    throw new NegativeAcknowledgeException();
                if (result == Message.Cancel)
                    throw new CancelledResponseException();

                var commandResult = result as NodeCommandCompleted;
                if (commandResult != null &&
                    commandResult.TransmissionState != TransmissionState.CompleteOk)
                    throw new TransmissionException($"Transmission failure:" +
                        $" {((NodeCommandCompleted)result).TransmissionState}.");

                if (predicate(result))
                {
                    return result;
                }
            }

            throw new TaskCanceledException();
        }

        private async Task<byte[]> Exchange(Func<Task<byte[]>> func,
            string message,
            CancellationToken cancellationToken)
        {
            if (func == null)
                throw new ArgumentNullException(nameof(func));

            await _semaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                var attempt = 0;
                while (!cancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        return await func().ConfigureAwait(false);
                    }
                    catch (CancelledResponseException)
                    {
                        if (attempt++ >= MaxRetryCount)
                            throw;

                        LogMessage($"Cancelled received on: {message}. Retrying attempt: {attempt}");

                        await Task.Delay(TimeSpan.FromMilliseconds(100), cancellationToken).
                            ConfigureAwait(false);
                    }
                    catch (TransmissionException)
                    {
                        if (attempt++ >= MaxRetryCount)
                            throw;

                        LogMessage($"Transmission failure on: {message}. Retrying attempt: {attempt}");

                        await Task.Delay(TimeSpan.FromMilliseconds(500), cancellationToken).
                            ConfigureAwait(false);
                    }
                    catch (TimeoutException)
                    {
                        if (attempt++ >= MaxRetryCount)
                            throw;

                        LogMessage($"Timeout on: {message}. Retrying attempt: {attempt}");

                        await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken).
                            ConfigureAwait(false);
                    }
                }
            }
            finally
            {
                _semaphore.Release();
            }

            throw new TaskCanceledException();
        }

        private async void ReadPort(ISerialPort port)
        {
            if (port == null)
                throw new ArgumentNullException(nameof(port));

            while (true)
            {
                try
                {
                    // wait for message received (blocking)
                    var message = await Message.Read(port.InputStream).ConfigureAwait(false);
                    LogMessage($"Received: {message}");

                    // ignore ACK, no processing of ACK needed
                    if (message == Message.Acknowledge)
                        continue;

                    // is it an event from a node?
                    var nodeEvent = message as NodeEvent;
                    if (nodeEvent != null)
                    {
                        // yes, so add to event queue
                        _eventQueue.Add(nodeEvent);
                        // send ACK to controller
                        _transmitQueue.Add(Message.Acknowledge);
                        // we're done
                        continue;
                    }

                    // is it an update from a node?
                    var nodeUpdate = message as NodeUpdate;
                    if (nodeUpdate != null)
                    {
                        // yes, so add to update queue
                        _updateQueue.Add(nodeUpdate);
                        // send ACK to controller
                        _transmitQueue.Add(Message.Acknowledge);
                        // we're done
                        continue;
                    }

                    // not a event, so it's a response to a request
                    _responseQueue.Add(message);
                    // send ACK to controller
                    _transmitQueue.Add(Message.Acknowledge);
                }
                catch (ChecksumException ex)
                {
                    LogMessage($"Exception: {ex}");
                    _transmitQueue.Add(Message.NegativeAcknowledge);
                }
                catch (UnknownFrameException ex)
                {
                    // probably out of sync on the serial port
                    // ToDo: handle gracefully 
                    OnError(new ErrorEventArgs(ex));
                }
                catch (IOException)
                {
                    // port closed, we're done so return
                    OnClosed(EventArgs.Empty);
                    return;
                }
                catch (Exception ex)
                {
                    // just raise error event. don't break reading of serial port
                    OnError(new ErrorEventArgs(ex));
                }
            }
        }

        private void ProcessQueue<T>(BlockingCollection<T> queue, Action<T> process) where T : Message
        {
            if (queue == null)
                throw new ArgumentNullException(nameof(queue));
            if (process == null)
                throw new ArgumentNullException(nameof(process));

            while (!queue.IsCompleted)
            {
                var message = default(Message);
                try
                {
                    message = queue.Take();
                }
                catch (InvalidOperationException)
                {
                }

                if (message != null)
                {
                    process((T)message);
                }
            }
        }

        private void LogMessage(string message)
        {
            if (message != null)
            {
                Console.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd H:mm:ss.fff")} {message}");
            }
        }

        private void HandleException(Exception ex)
        {
            if (ex is AggregateException)
            {
                foreach (var inner in ((AggregateException)ex).InnerExceptions)
                {
                    LogMessage(inner.ToString());
                }
                return;
            }
            LogMessage(ex.ToString());
        }

        private void OnNodeEventReceived(NodeEvent nodeEvent)
        {
            if (nodeEvent == null)
                throw new ArgumentNullException(nameof(nodeEvent));

            var handler = NodeEventReceived;
            if (handler != null)
            {
                foreach (var invocation in handler.GetInvocationList().Cast<EventHandler<NodeEventArgs>>())
                {
                    try
                    {
                        invocation(this, new NodeEventArgs(nodeEvent.NodeID, nodeEvent.Command));
                    }
                    catch (Exception ex)
                    {
                        LogMessage(ex.ToString());
                    }
                }
            }
        }

        private void OnNodeUpdateReceived(NodeUpdate nodeUpdate)
        {
            if (nodeUpdate == null)
                throw new ArgumentNullException(nameof(nodeUpdate));

            var handler = NodeUpdateReceived;
            if (handler != null)
            {
                foreach (var invocation in handler.GetInvocationList().Cast<EventHandler<NodeUpdateEventArgs>>())
                {
                    try
                    {
                        invocation(this, new NodeUpdateEventArgs(nodeUpdate.NodeID));
                    }
                    catch (Exception ex)
                    {
                        LogMessage(ex.ToString());
                    }
                }
            }
        }

        private void OnTransmit(Message message)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            message.Write(Port.OutputStream).ConfigureAwait(false);
            LogMessage($"Transmitted: {message}");
        }

        protected virtual void OnError(ErrorEventArgs e)
        {
            LogMessage($"Exception: {e.Error}");
            Error?.Invoke(this, e);
        }

        protected virtual void OnClosed(EventArgs e)
        {
            LogMessage($"Connection closed");
            Closed?.Invoke(this, e);
        }
    }
}
