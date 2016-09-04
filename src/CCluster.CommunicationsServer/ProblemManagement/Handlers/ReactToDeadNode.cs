using CCluster.CommunicationsServer.Notifications;
using MediatR;

namespace CCluster.CommunicationsServer.ProblemManagement.Handlers
{
    public class ReactToDeadNode : INotificationHandler<NodeRemoved>
    {
        private readonly IProblemManager manager;

        public ReactToDeadNode(IProblemManager manager)
        {
            this.manager = manager;
        }

        public void Handle(NodeRemoved notification)
        {
            manager.TaskManagerIsDead(notification.Id);
            manager.ComputationalNodeIsDead(notification.Id);
        }
    }
}
