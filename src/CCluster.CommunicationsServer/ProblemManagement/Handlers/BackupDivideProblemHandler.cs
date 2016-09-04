using CCluster.CommunicationsServer.Messaging;
using CCluster.Messages;
using MediatR;

namespace CCluster.CommunicationsServer.ProblemManagement.Handlers
{
    public class BackupDivideProblemHandler : INotificationHandler<BackupClientMessage<DivideProblem>>
    {
        private readonly IProblemManager manager;

        public BackupDivideProblemHandler(IProblemManager manager)
        {
            this.manager = manager;
        }

        public void Handle(BackupClientMessage<DivideProblem> notification)
        {
            manager.MarkAsDuringDivide(notification.Message.Id, notification.Message.NodeID);
        }
    }
}
