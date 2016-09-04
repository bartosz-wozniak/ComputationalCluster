using CCluster.Common;
using CCluster.CommunicationsServer.Messaging;
using CCluster.CommunicationsServer.Notifications;
using CCluster.Messages.Register;
using log4net;
using MediatR;

namespace CCluster.CommunicationsServer.Backup.Handlers
{
    public class StopTrackingNode : INotificationHandler<BackupClientMessage<RegisterMessage>>
    {
        private readonly ILog logger = LogProvider.GetCurrentClassLogger();

        private readonly IMediator mediator;

        public StopTrackingNode(IMediator mediator)
        {
            this.mediator = mediator;
        }

        public void Handle(BackupClientMessage<RegisterMessage> notification)
        {
            if (notification.Message.Deregister)
            {
                logger.Debug($"Node {notification.Message.Id} deregistered in primary CS, marking as dead.");
                mediator.Publish(new NodeDead(notification.Message.Id));
            }
        }
    }
}
