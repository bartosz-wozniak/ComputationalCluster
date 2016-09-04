using MediatR;

namespace CCluster.CommunicationsServer.Notifications
{
    public class NodeDead : INotification
    {
        public ulong Id { get; }

        public NodeDead(ulong id)
        {
            this.Id = id;
        }
    }
}
