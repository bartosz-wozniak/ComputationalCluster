using System.Collections.Generic;

namespace CCluster.Common.NodeBackup
{
    public interface IBackupManager
    {
        void UpdateServers(IEnumerable<BackupServerInfo> servers);
        BackupServerInfo GetNextServer();
    }
}