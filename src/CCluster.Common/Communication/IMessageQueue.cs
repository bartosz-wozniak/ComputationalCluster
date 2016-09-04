using CCluster.Messages;

namespace CCluster.Common.Communication
{
    public interface IMessageQueue
    {
        void Enqueue(IMessage msg);
    }
}
