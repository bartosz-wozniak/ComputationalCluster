using System.Net;

namespace CCluster.CommunicationsServer.NodeTrack
{
    public interface INodeTracker
    {
        void DiscardOutdatedNodes();
    }
}
