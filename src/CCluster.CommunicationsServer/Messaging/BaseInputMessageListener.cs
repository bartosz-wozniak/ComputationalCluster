using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using CCluster.Common;
using CCluster.Common.Communication;
using CCluster.CommunicationsServer.Exceptions;
using log4net;
using MediatR;

namespace CCluster.CommunicationsServer.Messaging
{
    public abstract class BaseInputMessageListener : SoftThread
    {
        protected readonly ILog logger = LogProvider.GetCurrentClassLogger();

        protected readonly IMediator mediator;
        private readonly ITcpListener listener;

        public BaseInputMessageListener(IMediator mediator, int port)
        {
            this.mediator = mediator;

            listener = new TcpListenerWrapper(new TcpListener(IPAddress.Any, port));
        }

        public override void Start()
        {
            logger.Info("Starting input thread.");
            listener.Start();
            base.Start();
        }

        public override void Stop()
        {
            logger.Info("Stopping input thread");
            base.Stop();
        }

        protected override void PreStopEvent()
        {
            listener.Dispose();
        }

        protected abstract IEnumerable<IRequest> ReadAvailableMessages(ITcpClient tcpClient);

        protected override void ThreadMain(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                ITcpClient tcpClient;

                try
                {
                    tcpClient = listener.AcceptTcpClient();
                }
                catch (SocketException ex) when (ex.SocketErrorCode == SocketError.Interrupted)
                {
                    return; // Thread has been stopped during Accept
                }
                catch (Exception ex)
                {
                    logger.Fatal("Cannot accept TCPClient.", ex);
                    throw;
                }

                if (token.IsCancellationRequested)
                {
                    tcpClient.Dispose();
                    return;
                }

                try
                {
                    var msgs = ReadAvailableMessages(tcpClient);
                    foreach (var msg in msgs)
                    {
                        mediator.Send(msg);
                    }
                }
                catch (CannotReadMessageException ex)
                {
                    logger.Error("Cannot read message from the client: " + ex.Message);
                    tcpClient.Dispose();
                }
            }
        }
    }
}
