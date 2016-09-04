using System.Net;

namespace CCluster.CommunicationsServer.Backup
{
    public sealed class BackupNodeInfo
    {
        public ulong Id { get; }
        public IPAddress Address { get; }
        public int Port { get; }

        public BackupNodeInfo(ulong id, IPAddress address, int port)
        {
            Id = id;
            Address = address;
            Port = port;
        }
    }
}
