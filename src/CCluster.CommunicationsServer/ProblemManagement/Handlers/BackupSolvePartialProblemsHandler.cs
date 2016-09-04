using System.Linq;
using CCluster.CommunicationsServer.Messaging;
using CCluster.Messages;
using MediatR;

namespace CCluster.CommunicationsServer.ProblemManagement.Handlers
{
    public class BackupSolvePartialProblemsHandler : INotificationHandler<BackupClientMessage<SolvePartialProblems>>
    {
        private readonly IProblemManager manager;

        public BackupSolvePartialProblemsHandler(IProblemManager manager)
        {
            this.manager = manager;
        }

        public void Handle(BackupClientMessage<SolvePartialProblems> notification)
        {
            var nodeId = notification.Message.PartialProblems[0].NodeID;
            var tasks = notification.Message.PartialProblems.Select(t => t.TaskId);

            manager.MarkAsDuringPartialProblemSolving(notification.Message.Id, nodeId, tasks);
        }
    }
}
