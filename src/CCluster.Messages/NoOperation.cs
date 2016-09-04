using CCluster.Messages.Register;

namespace CCluster.Messages
{
    public class NoOperation : IMessage
    {
        public BackupServer[] BackupServers { get; set; }
    }
}
