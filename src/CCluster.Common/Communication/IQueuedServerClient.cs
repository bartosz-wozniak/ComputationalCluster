using CCluster.Messages;

namespace CCluster.Common.Communication
{
    public interface IQueuedServerClient
    {
        void Send(IMessage msg);
    }
}
