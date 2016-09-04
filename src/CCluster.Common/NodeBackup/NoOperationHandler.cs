using System.Linq;
using System.Net;
using CCluster.Messages;
using CCluster.Messages.Register;
using log4net;
using MediatR;

namespace CCluster.Common.NodeBackup
{
    public class NoOperationHandler : RequestHandler<NoOperation>
    {
        private readonly ILog logger = LogProvider.GetCurrentClassLogger();

        private readonly IBackupManager backupManager;

        public NoOperationHandler(IBackupManager backupManager)
        {
            this.backupManager = backupManager;
        }

        protected override void HandleCore(NoOperation message)
        {
            logger.Info("CS sent us NoOperation message.");

            // TODO what happens when Address is not an IP address? Should we ignore the exception or handle it
            // gracefully?
            var servers = message.BackupServers ?? new BackupServer[0];
            backupManager.UpdateServers(
                servers.Select(s => new BackupServerInfo(s.Id, IPAddress.Parse(s.Address), s.Port))
            );
        }
    }
}
