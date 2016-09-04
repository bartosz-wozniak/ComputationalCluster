using System.Collections.Generic;
using System.Linq;

namespace CCluster.Common.NodeBackup
{
    public class BackupManager : IBackupManager
    {
        private readonly object lockObj = new object();

        private List<BackupServerInfo> servers = new List<BackupServerInfo>();

        public void UpdateServers(IEnumerable<BackupServerInfo> servers)
        {
            lock (lockObj)
            {
                this.servers = servers.ToList();
                this.servers.Sort((a, b) => a.Id.CompareTo(b.Id));
            }
        }

        public BackupServerInfo GetNextServer()
        {
            lock (lockObj)
            {
                if (servers.Count > 0)
                {
                    var fst = servers[0];
                    servers.RemoveAt(0);
                    return fst;
                }
                return null;
            }
        }
    }
}
