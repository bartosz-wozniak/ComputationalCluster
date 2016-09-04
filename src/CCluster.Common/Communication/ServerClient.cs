using System;
using System.Collections.Generic;
using System.Net;
using CCluster.Common.Communication.Exceptions;
using CCluster.Common.Communication.Messaging;
using CCluster.Common.Configuration;
using CCluster.Messages;
using log4net;
using MediatR;

namespace CCluster.Common.Communication
{
    public class ServerClient : IServerClient
    {
        private const int StreamTimeout = 1000;

        private readonly ILog logger = LogProvider.GetCurrentClassLogger();

        private readonly ICsConfiguration config;
        private readonly IMessagesSender sender;
        private readonly IMediator mediator;
        private readonly Func<IPEndPoint, ITcpClient> tcpClientFactory;
        private readonly Func<INetworkStream, IMessageStreamReader> readerFactory;

        private readonly IPEndPoint endPoint;

        public ServerClient(ICsConfiguration config, IMessagesSender sender, IMediator mediator,
            Func<ITcpClient> tcpClientFactory, Func<INetworkStream, IMessageStreamReader> readerFactory)
        {
            this.config = config;
            this.sender = sender;
            this.mediator = mediator;
            this.tcpClientFactory = _ => tcpClientFactory();
            this.readerFactory = readerFactory;
        }

        public ServerClient(ICsConfiguration config, IMessagesSender sender, IMediator mediator,
           Func<IPEndPoint, ITcpClient> tcpClientFactory, Func<INetworkStream, IMessageStreamReader> readerFactory,
           IPEndPoint endPoint)
        {
            this.config = config;
            this.sender = sender;
            this.mediator = mediator;
            this.tcpClientFactory = tcpClientFactory;
            this.readerFactory = readerFactory;
            this.endPoint = endPoint;
        }

        public void Send(IMessage msg)
        {
            logger.Debug($"Sending {msg} to CS.");
            using (var client = tcpClientFactory(endPoint))
            {
                TryConnect(client);

                using (var stream = client.GetStream())
                {
                    stream.Timeout = StreamTimeout;
                    TrySend(msg, stream);

                    ReceiveMessageIfNeeded(stream, msg);
                }
            }
        }

        private void ReceiveMessageIfNeeded(INetworkStream stream, IMessage msg)
        {
            logger.Debug("Receiving messages.");
            if (!(msg is INoResponseMessage))
            {
                var reader = readerFactory(stream);
                var responseMessages = TryReadResponse(reader);
                if (responseMessages.Count == 0)
                {
                    logger.Error("CS sent us no response, throwing.");
                    throw new NoResponseException();
                }
                foreach (var response in responseMessages)
                {
                    mediator.Send(response);
                }
            }
        }

        private void TryConnect(ITcpClient client)
        {
            logger.Debug("Connecting to CS.");
            try
            {
                client.Connect(config.Address, config.Port);
            }
            catch (Exception ex)
            {
                logger.Error("Connection failed, throwing CannotSendMessageException, exception was caught: " + ex.Message);
                throw new CannotSendMessageException("Cannot connect to the server.", ex);
            }
        }

        private void TrySend(IMessage msg, INetworkStream stream)
        {
            try
            {
                sender.Send(new[] { msg }, stream);
            }
            catch (Exception ex)
            {
                logger.Error("Sending failed, re-throwing CannotSendMessageException.", ex);
                throw new CannotSendMessageException("Cannot send message to the server.", ex);
            }
        }

        private IReadOnlyList<IMessage> TryReadResponse(IMessageStreamReader reader)
        {
            try
            {
                return reader.ReadAll();
            }
            catch (Exception ex)
            {
                logger.Error("Reading failed, re-throwing CannotSendMessageException.", ex);
                throw new CannotSendMessageException("Cannot read response from the server.", ex);
            }
        }
    }
}
