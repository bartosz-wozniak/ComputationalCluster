using System.Net;

namespace CCluster.Common.NodeBackup
{
    public sealed class BackupServerInfo
    {
        public ulong Id { get; }
        public IPAddress Address { get; }
        public int Port { get; }

        public BackupServerInfo(ulong id, IPAddress address, int port)
        {
            Id = id;
            Address = address;
            Port = port;
        }
    }
}
