using System.Linq;
using CCluster.Common;
using CCluster.CommunicationsServer.Notifications;
using CCluster.Messages;
using log4net;
using MediatR;

namespace CCluster.CommunicationsServer.NodeTrack
{
    public class NodeTracker : INodeTracker,
        INotificationHandler<StatusMessage>, INotificationHandler<NodeRegistered>,
        INotificationHandler<SwitchedToPrimary>
    {
        private readonly ILog logger = LogProvider.GetCurrentClassLogger();

        private readonly ICsDataStore csDataStore;
        private readonly ITimeProvider timeProvider;
        private readonly IMediator mediator;
        private readonly CommunicationsServerConfiguration communicationsServerConfiguration;

        public NodeTracker(ICsDataStore csDataStore, ITimeProvider timeProvider, IMediator mediator,
            CommunicationsServerConfiguration communicationsServerConfiguration)
        {
            this.csDataStore = csDataStore;
            this.timeProvider = timeProvider;
            this.mediator = mediator;
            this.communicationsServerConfiguration = communicationsServerConfiguration;
        }

        public void DiscardOutdatedNodes()
        {
            logger.Debug("Discarding outdated nodes");
            var now = timeProvider.Now();

            var toRemove = csDataStore.ConnectedNodes
                .Where(n => now - n.LastStatusMessageTime > communicationsServerConfiguration.CommunicationsTimeoutTimeSpan)
                .ToList();

            foreach (var n in toRemove)
            {
                logger.Warn($"Node {n.Id} discarded as not responding.");
                csDataStore.RemoveNode(n);
                mediator.Publish(new NodeRemoved(n.Id));
            }
        }

        public void Handle(StatusMessage notification)
        {
            logger.Debug($"Updating lastseen time for node {notification.Id}.");
            var node = csDataStore.GetById(notification.Id);
            if (node != null)
            {
                node.LastStatusMessageTime = timeProvider.Now();
            }
        }

        public void Handle(NodeRegistered notification)
        {
            logger.Debug($"Registering new node {notification.Message.Id}.");
            csDataStore.AddNode(notification.Message.Id, notification.Message.Type,
                notification.Message.SolvableProblems);
        }

        public void Handle(SwitchedToPrimary notification)
        {
            foreach (var item in csDataStore.ConnectedNodes)
            {
                item.LastStatusMessageTime = timeProvider.Now();
            }
        }
    }
}
