using System;
using System.Collections.Generic;
using System.Linq;
using CCluster.Common.Communication;
using CCluster.Common.Communication.Messaging;
using MediatR;

namespace CCluster.CommunicationsServer.Messaging
{
    public class BackupInputMessageListener : BaseInputMessageListener
    {
        private readonly Func<INetworkStream, IMessageStreamReader> streamReaderFactory;

        public BackupInputMessageListener(IMediator mediator, CommunicationsServerConfiguration cfg,
            Func<INetworkStream, IMessageStreamReader> streamReaderFactory)
            : base(mediator, cfg.Port)
        {
            this.streamReaderFactory = streamReaderFactory;
        }

        protected override IEnumerable<IRequest> ReadAvailableMessages(ITcpClient tcpClient)
        {
            using (tcpClient)
            {
                var address = tcpClient.RemoteEndpoint;
                var streamReader = streamReaderFactory(tcpClient.GetStream());
                var msgs = streamReader.ReadAll();
                return msgs.Select(m => new BackupClientMessage(m, address));
            }
        }
    }
}
