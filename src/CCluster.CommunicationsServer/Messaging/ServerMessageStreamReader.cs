using System;
using CCluster.Common;
using CCluster.Common.Communication;
using CCluster.Common.Communication.Messaging;
using CCluster.CommunicationsServer.Exceptions;
using CCluster.Messages;

namespace CCluster.CommunicationsServer.Messaging
{
    public class ServerMessageStreamReader
    {
        private const int BufferSize = Constants.BufferSize;
        private const int Timeout = 2000;

        private readonly INetworkStream stream;
        private readonly IMessageDeserializer deserializer;

        private IMessage message;

        private readonly byte[] buffer = new byte[BufferSize];
        private int bufferPos;

        public ServerMessageStreamReader(INetworkStream stream, IMessageDeserializer deserializer)
        {
            this.stream = stream;
            this.deserializer = deserializer;
        }

        public IMessage ReadAvailable()
        {
            stream.Timeout = Timeout;

            try
            {
                int read;
                while ((read = stream.Read(buffer, bufferPos, buffer.Length - bufferPos)) > 0)
                {
                    bufferPos += read;
                    if (TryToDeserialize())
                    {
                        return message;
                    }
                }
            }
            catch (Exception)
            {
                throw new CannotReadMessageException();
            }

            throw new CannotReadMessageException();
        }

        private bool TryToDeserialize()
        {
            try
            {
                message = deserializer.Deserialize(buffer, 0, bufferPos);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }
    }
}