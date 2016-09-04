using System;
using System.Collections.Generic;
using CCluster.Common.Communication;
using CCluster.CommunicationsServer.NodeTrack;
using MediatR;

namespace CCluster.CommunicationsServer.Messaging
{
    public class PrimaryInputMessageListener : BaseInputMessageListener
    {
        private readonly INodeTracker nodeTracker;
        private readonly Func<INetworkStream, ServerMessageStreamReader> streamReaderFactory;

        public PrimaryInputMessageListener(IMediator mediator, CommunicationsServerConfiguration cfg,
            INodeTracker nodeTracker, Func<INetworkStream, ServerMessageStreamReader> srf)
            : base(mediator, cfg.Port)
        {
            this.nodeTracker = nodeTracker;
            streamReaderFactory = srf;
        }

        protected override IEnumerable<IRequest> ReadAvailableMessages(ITcpClient tcpClient)
        {
            var streamReader = streamReaderFactory(tcpClient.GetStream());
            var msg = streamReader.ReadAvailable();
            yield return ClientMessage.Create(msg, tcpClient);
        }
    }
}
