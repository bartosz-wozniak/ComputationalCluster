using CCluster.Common;
using CCluster.CommunicationsServer.Messaging;
using CCluster.CommunicationsServer.Notifications;
using CCluster.Messages.Register;
using log4net;
using MediatR;

namespace CCluster.CommunicationsServer.Backup.Handlers
{
    public class StartTrackingNewNode : INotificationHandler<BackupClientMessage<RegisterMessage>>
    {
        private readonly ILog logger = LogProvider.GetCurrentClassLogger();

        private readonly IMediator mediator;

        public StartTrackingNewNode(IMediator mediator)
        {
            this.mediator = mediator;
        }

        public void Handle(BackupClientMessage<RegisterMessage> notification)
        {
            if (!notification.Message.Deregister)
            {
                logger.Debug($"New node {notification.Message.Id} registered in primary CS, adding it here.");
                mediator.Publish(new NodeRegistered(notification.Message, notification.MessageSource));
            }
        }
    }
}
