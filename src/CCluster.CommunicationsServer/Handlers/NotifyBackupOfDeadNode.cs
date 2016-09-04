using CCluster.Common;
using CCluster.CommunicationsServer.Backup;
using CCluster.CommunicationsServer.Notifications;
using CCluster.Messages.Register;
using log4net;
using MediatR;

namespace CCluster.CommunicationsServer.Handlers
{
    public class NotifyBackupOfDeadNode : INotificationHandler<NodeRemoved>
    {
        private readonly ILog logger = LogProvider.GetCurrentClassLogger();

        private readonly IBackupSender sender;

        public NotifyBackupOfDeadNode(IBackupSender sender)
        {
            this.sender = sender;
        }

        public void Handle(NodeRemoved notification)
        {
            logger.Info($"Node {notification.Id} marked as removed.");
            sender.Send(new RegisterMessage
            {
                Id = notification.Id,
                Type = "NOT_SUPPORTED_HERE",
                Deregister = true
            });
        }
    }
}
