using CCluster.Messages;

namespace CCluster.CommunicationsServer.Backup
{
    public interface IBackupSender
    {
        void Send(IMessage message);
    }
}