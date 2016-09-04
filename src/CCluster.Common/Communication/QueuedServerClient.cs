using System;
using CCluster.Common;
using CCluster.Messages;
using log4net;

namespace CCluster.Common.Communication
{
    public class QueuedServerClient : IQueuedServerClient
    {
        private readonly ILog logger = LogProvider.GetCurrentClassLogger();

        private readonly IServerClient serverClient;
        private readonly IMessageQueue msgQueue;

        public QueuedServerClient(IServerClient servereClient, IMessageQueue msgQueue)
        {
            this.serverClient = servereClient;
            this.msgQueue = msgQueue;
        }

        // TODO decide if we really want to send all the messages like this
        public void Send(IMessage msg)
        {
            try
            {
                serverClient.Send(msg);
            }
            catch (Exception ex)
            {
                logger.Error($"Cannot send message {msg} to CS, queueing.", ex);
                msgQueue.Enqueue(msg);
            }
        }
    }
}
