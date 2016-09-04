using CCluster.Common.Communication.Messaging;
using CCluster.CommunicationsServer.Backup;
using CCluster.Messages;

namespace CCluster.CommunicationsServer.ProblemManagement.Handlers
{
    public class SolveRequestHandler : BaseProblemHandler<SolveRequest>
    {
        public SolveRequestHandler(IProblemManager manager, IMessagesSender sender, IBackupSender backupSender)
            : base(manager, sender, backupSender)
        { }

        protected override void HandleBackup(SolveRequest message)
        {
            manager.AddProblem(new ProblemDefinition(
                type: message.ProblemType,
                data: message.Data),
                message.Id);
        }

        protected override IMessage HandlePrimary(SolveRequest message)
        {
            var newId = manager.AddProblem(new ProblemDefinition(
                type: message.ProblemType,
                data: message.Data));
            return new SolveRequestResponse { Id = newId };
        }
    }
}
