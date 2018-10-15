﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ZWaveCore.Core.Exceptions;
using ZWaveCore.Enums;

namespace ZWaveCore.Core
{
    class Message : IEquatable<Message>
    {
        public static readonly Message Acknowledge = new Message(FrameHeader.Acknowledge);
        public static readonly Message NegativeAcknowledge = new Message(FrameHeader.NegativeAcknowledge);
        public static readonly Message Cancel = new Message(FrameHeader.Cancel);

        public readonly FrameHeader Header;
        public readonly MessageType? Type;
        public readonly Function? Function;

        protected Message(FrameHeader header, MessageType? type = null, Function? function = null)
        {
            Header = header;
            Type = type;
            Function = function;
        }

        public override string ToString()
        {
            return $"{Header} {Type} {Function}";
        }

        protected virtual List<byte> GetPayload()
        {
            var buffer = new List<byte>();
            buffer.Add((byte)FrameHeader.StartOfFrame);
            buffer.Add(0x00);
            buffer.Add((byte)Type);
            buffer.Add((byte)Function);
            return buffer;
        }

        public Task Write(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            if (Header == FrameHeader.StartOfFrame)
            {
                var payload = GetPayload();

                // update length
                payload[1] = (byte)(payload.Count - 1);

                // add checksum 
                payload.Add(payload.Skip(1).Aggregate((byte)0xFF, (total, next) => total ^= next));

                return stream.WriteAsync(payload.ToArray(), 0, payload.Count);
            }

            switch (Header)
            {
                case FrameHeader.Acknowledge:
                    return stream.WriteAsync(new[] { (byte)FrameHeader.Acknowledge }, 0, 1);
                case FrameHeader.NegativeAcknowledge:
                    return stream.WriteAsync(new[] { (byte)FrameHeader.NegativeAcknowledge }, 0, 1);
                case FrameHeader.Cancel:
                    return stream.WriteAsync(new[] { (byte)FrameHeader.Cancel }, 0, 1);
            }

            throw new NotSupportedException("Frameheader is not supported");
        }

        public static async Task<Message> Read(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            var buffer = new byte[1024];
            await stream.ReadAsyncExact(buffer, 0, 1);
            var header = (FrameHeader)buffer[0];

            switch (header)
            {
                case FrameHeader.Acknowledge:
                    return Message.Acknowledge;
                case FrameHeader.NegativeAcknowledge:
                    return Message.NegativeAcknowledge;
                case FrameHeader.Cancel:
                    return Message.Cancel;
            }

            if (header == FrameHeader.StartOfFrame)
            {
                await stream.ReadAsyncExact(buffer, 1, 1);
                var length = buffer[1];

                buffer = buffer.Take(length + 2).ToArray();
                await stream.ReadAsyncExact(buffer, 2, length);

                var type = (MessageType)buffer[2];
                var function = (Function)buffer[3];
                var payload = buffer.Skip(4).Take(length - 3).ToArray();

                if (buffer.Skip(1).Take(buffer.Length - 2).Aggregate((byte)0xFF, (total, next) => (byte)(total ^ next)) != buffer[buffer.Length - 1])
                    throw new ChecksumException("Checksum failure");

                if (type == MessageType.Request)
                {
                    if (function == Enums.Function.ApplicationCommandHandler)
                    {
                        return new NodeEvent(payload);
                    }
                    if (function == Enums.Function.SendData)
                    {
                        return new NodeCommandCompleted(payload);
                    }
                    if (function == Enums.Function.ApplicationUpdate)
                    {
                        return new NodeUpdate(payload);
                    }
                    else
                    {
                        return new ControllerFunctionEvent(function, payload);
                    }
                }
                if (type == MessageType.Response)
                {
                    if (function != Enums.Function.SendData)
                    {
                        return new ControllerFunctionCompleted(function, payload);
                    }
                }
                return new UnknownMessage(header, type, function, payload);
            }

            throw new UnknownFrameException($"Frame {header} is not supported");
        }

        public override int GetHashCode()
        {
            return Header.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Message);
        }

        public bool Equals(Message other)
        {
            if (Object.ReferenceEquals(other, null))
                return false;

            if (Object.ReferenceEquals(other, this))
                return true;

            if (GetType() != other.GetType())
                return false;

            return object.Equals(Header, other.Header) && object.Equals(Type, other.Type) && object.Equals(Function, other.Function);
        }

        public static bool operator ==(Message a, Message b)
        {
            if (Object.ReferenceEquals(a, b))
                return true;

            if (Object.ReferenceEquals(a, null) || Object.ReferenceEquals(b, null))
                return false;

            return object.Equals(a, b);
        }

        public static bool operator !=(Message a, Message b)
        {
            return !(a == b);
        }
    }

    static partial class Extensions
    {
        public static async Task ReadAsyncExact(this Stream stream, byte[] buffer, int offset, int count)
        {
            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));

            var read = 0;
            while (read < count)
            {
                read += await stream.ReadAsync(buffer, offset + read, count - read);
            }
        }
    }
}
