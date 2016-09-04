using CCluster.CommunicationsServer.Messaging;
using CCluster.Messages;
using MediatR;

namespace CCluster.CommunicationsServer.ProblemManagement.Handlers
{
    public class BackupProblemSync : INotificationHandler<BackupClientMessage<ProblemSync>>
    {
        private readonly IProblemManager manager;

        public BackupProblemSync(IProblemManager manager)
        {
            this.manager = manager;
        }

        public void Handle(BackupClientMessage<ProblemSync> notification)
        {
            manager.AddProblem(notification.Message);
        }
    }
}
