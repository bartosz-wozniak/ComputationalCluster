using System.Collections.Generic;
using CCluster.Messages.Register;

namespace CCluster.CommunicationsServer.Backup
{
    public interface IBackupServerManager
    {
        IReadOnlyList<BackupNodeInfo> BackupServers { get; }

        BackupNodeInfo GetNextPrimaryServer();
        IReadOnlyList<BackupNodeInfo> GetFollowingNodes();

        void UpdateList(IEnumerable<BackupServer> servers);
    }
}