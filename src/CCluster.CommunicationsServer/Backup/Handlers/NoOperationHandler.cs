using CCluster.CommunicationsServer.Backup;
using CCluster.Messages;
using MediatR;

namespace CCluster.CommunicationsServer.Backup.Handlers
{
    public class NoOperationHandler : RequestHandler<NoOperation>
    {
        private readonly IBackupServerManager backupServers;

        public NoOperationHandler(IBackupServerManager backupServers)
        {
            this.backupServers = backupServers;
        }

        protected override void HandleCore(NoOperation message)
        {
            backupServers.UpdateList(message.BackupServers);
        }
    }
}
