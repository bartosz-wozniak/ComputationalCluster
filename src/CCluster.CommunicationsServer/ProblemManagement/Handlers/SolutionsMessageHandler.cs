using System.Linq;
using CCluster.Common;
using CCluster.Common.Communication.Messaging;
using CCluster.CommunicationsServer.Backup;
using CCluster.CommunicationsServer.NodeTrack;
using CCluster.Messages;
using CCluster.Messages.Register;

namespace CCluster.CommunicationsServer.ProblemManagement.Handlers
{
    public class SolutionsMessageHandler : BaseProblemHandler<Solutions>
    {
        private readonly IBackupServerManager backups;
        private readonly ICsDataStore nodeStore;

        public SolutionsMessageHandler(IProblemManager manager, IMessagesSender sender, IBackupServerManager backups,
            IBackupSender backupSender, ICsDataStore nodeStore)
            : base(manager, sender, backupSender)
        {
            this.backups = backups;
            this.nodeStore = nodeStore;
        }

        protected override IMessage HandlePrimary(Solutions message)
        {
            if (IsDivideResponse(message))
            {
                HandleDivideResponse(message);
            }
            else if (IsMergeResponse(message))
            {
                HandleMergeResponse(message);
            }
            else
            {
                HandlePartialResults(message);
            }
            return GetNoOpMessage();
        }

        protected override void HandleBackup(Solutions message)
        {
            if (IsMergeRequest(message))
            {
                HandleMergeRequest(message);
            }
            else
            {
                HandlePrimary(message);
            }
        }

        private bool IsDivideResponse(Solutions message)
        {
            return message.SolutionsList.All(s => s.Type == SolutionType.Ongoing);
        }

        private bool IsMergeRequest(Solutions message)
        {
            if (message.SolutionsList.Length > 0 && message.SolutionsList.All(s => s.Type == SolutionType.Partial))
            {
                var id = message.SolutionsList[0].NodeID;
                var node = nodeStore.GetById(id);
                return node?.Type == Constants.NodeTypes.TaskManager;
            }
            return false;
        }

        private bool IsMergeResponse(Solutions message)
        {
            return message.SolutionsList.Length == 1 && message.SolutionsList[0].Type == SolutionType.Final;
        }

        private void HandleDivideResponse(Solutions message)
        {
            manager.MarkAsDivided(message.Id,
                message.SolutionsList.Select(s => new SubProblemDefinition(s.TaskId, s.Data))
            );
        }

        private void HandleMergeResponse(Solutions message)
        {
            manager.MarkAsCompleted(message.Id, message.SolutionsList[0].Data);
        }

        private void HandlePartialResults(Solutions message)
        {
            manager.MarkSubProblemsAsSolved(message.Id,
                message.SolutionsList.Select(s => new SubProblemSolution(s.TaskId, s.Data))
            );
        }

        private void HandleMergeRequest(Solutions message)
        {
            manager.MarkAsDuringMerge(message.Id, message.SolutionsList[0].NodeID);
        }

        private NoOperation GetNoOpMessage()
        {
            var servers = backups.BackupServers.Select(s =>
                new BackupServer
                {
                    Id = s.Id,
                    Address = s.Address.ToString(),
                    Port = s.Port
                })
                .ToArray();

            return new NoOperation { BackupServers = servers };
        }
    }
}
