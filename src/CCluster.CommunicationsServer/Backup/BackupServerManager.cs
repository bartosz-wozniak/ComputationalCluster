using System.Collections.Generic;
using System.Linq;
using System.Net;
using CCluster.Common;
using CCluster.CommunicationsServer.Notifications;
using CCluster.Messages.Notifications;
using CCluster.Messages.Register;
using MediatR;

namespace CCluster.CommunicationsServer.Backup
{
    /// <summary>
    /// This has two responsibilities - works as "normal" backup manager and as a backup manager for CS. This is bad,
    /// but was faster to write.
    /// </summary>
    public class BackupServerManager : IBackupServerManager,
        INotificationHandler<NodeRegistered>, INotificationHandler<NodeRemoved>,
        INotificationHandler<Registered>, INotificationHandler<SwitchedToPrimary>
    {
        private readonly object lockObj = new object();
        private readonly SortedList<ulong, BackupNodeInfo> backupServers = new SortedList<ulong, BackupNodeInfo>();

        private ulong ourId = 0;

        public IReadOnlyList<BackupNodeInfo> BackupServers
        {
            get
            {
                lock (lockObj)
                {
                    return backupServers.Values.ToArray();
                }
            }
        }

        public BackupNodeInfo GetNextPrimaryServer()
        {
            lock (lockObj)
            {
                if (backupServers.Count == 0)
                {
                    return null;
                }

                var first = backupServers.Values[0];
                backupServers.RemoveAt(0);
                if (first.Id == ourId)
                {
                    return null;
                }
                return first;
            }
        }

        public IReadOnlyList<BackupNodeInfo> GetFollowingNodes()
        {
            lock (lockObj)
            {
                return backupServers.Values.Where(v => v.Id > ourId).ToArray();
            }
        }

        public void UpdateList(IEnumerable<BackupServer> servers)
        {
            lock (lockObj)
            {
                backupServers.Clear();
                foreach (var server in servers)
                {
                    backupServers.Add(server.Id, new BackupNodeInfo(
                        id: server.Id,
                        address: IPAddress.Parse(server.Address),
                        port: server.Port));
                }
            }
        }

        public void Handle(NodeRegistered notification)
        {
            if (notification.Message.Type == Constants.NodeTypes.CommunicationsServer)
            {
                lock (lockObj)
                {
                    if (!backupServers.ContainsKey(notification.Message.Id))
                    {
                        backupServers.Add(
                            notification.Message.Id,
                            new BackupNodeInfo(
                            id: notification.Message.Id,
                            address: notification.MessageSource.Address,
                            port: notification.MessageSource.Port
                        ));
                    }
                }
            }
        }

        public void Handle(NodeRemoved notification)
        {
            lock (lockObj)
            {
                backupServers.Remove(notification.Id);
            }
        }

        public void Handle(Registered notification)
        {
            ourId = notification.AssignedId;
        }

        public void Handle(SwitchedToPrimary notification)
        {
            lock (lockObj)
            {
                backupServers.Remove(ourId);
            }
            ourId = 0;
        }
    }
}
