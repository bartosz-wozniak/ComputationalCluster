using System;
using System.Collections.Generic;
using CCluster.Common;
using CCluster.Messages;
using log4net;

namespace CCluster.Common.Communication.Messaging
{
    public sealed class MessageStreamReader : IMessageStreamReader
    {
        private const int BufferSize = Constants.BufferSize;
        private static readonly IReadOnlyList<IMessage> EmptyList = new IMessage[0];

        private readonly IMessageDeserializer deserializer;
        private readonly INetworkStream inputStream;

        private static readonly ILog log = LogProvider.GetCurrentClassLogger();

        private int bufferPos = 0;
        private readonly byte[] buffer = new byte[BufferSize];

        public bool MayHaveMessages
        {
            get
            {
                return inputStream.Connected || inputStream.DataAvailable || bufferPos > 0;
            }
        }

        public MessageStreamReader(IMessageDeserializer deserializer, INetworkStream inputStream)
        {
            log.Info("Create message stream reader");
            this.deserializer = deserializer;
            this.inputStream = inputStream;
        }

        public IReadOnlyList<IMessage> ReadAvailable()
        {
            bool hasRead = false;
            if (inputStream.DataAvailable)
            {
                int read = inputStream.Read(buffer, bufferPos, buffer.Length - bufferPos);
                bufferPos += read;
                hasRead = read > 0;
            }

            if (!inputStream.Connected || hasRead)
            {
                return ReadMessages();
            }
            return EmptyList;
        }

        private IReadOnlyList<IMessage> ReadMessages()
        {
            List<IMessage> messages = new List<IMessage>();
            var lastIdx = ParseSeparatedMessages(messages);
            if (!inputStream.Connected)
            {
                ParseLastMessage(messages, lastIdx);
            }
            else
            {
                AdjustBuffer(lastIdx);
            }
            return messages;
        }

        private int ParseSeparatedMessages(List<IMessage> messages)
        {
            int lastIdx = -1;
            int idx = -1;
            while ((idx = LocateSeparator(lastIdx + 1)) != -1)
            {
                int len = idx - lastIdx - 1;
                var msg = deserializer.Deserialize(buffer, lastIdx + 1, len);
                messages.Add(msg);
                lastIdx = idx;
            }
            return lastIdx;
        }

        private void ParseLastMessage(List<IMessage> messages, int lastIdx)
        {
            if (lastIdx + 1 != bufferPos)
            {
                var len = bufferPos - lastIdx - 1;
                var msg = deserializer.Deserialize(buffer, lastIdx + 1, len);
                messages.Add(msg);
            }
            bufferPos = 0;
        }

        private void AdjustBuffer(int lastIdx)
        {
            var len = bufferPos - lastIdx - 1;
            Buffer.BlockCopy(buffer, lastIdx + 1, buffer, 0, len);
            bufferPos = len;
        }

        private int LocateSeparator(int offset)
        {
            return Array.IndexOf(buffer, Constants.MessageSeparator, offset, bufferPos - offset);
        }
    }
}
