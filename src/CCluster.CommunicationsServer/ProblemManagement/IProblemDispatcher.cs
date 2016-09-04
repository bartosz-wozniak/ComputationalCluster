using CCluster.Messages;

namespace CCluster.CommunicationsServer.ProblemManagement
{
    public interface IProblemDispatcher
    {
        IMessage GetWorkForNode(ulong nodeId);
    }
}
