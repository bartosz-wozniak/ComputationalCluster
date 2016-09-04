using CCluster.Messages;

namespace CCluster.Common.Communication.Messaging
{
    /// <summary>
    /// Serializes a single message.
    /// </summary>
    public interface IMessageSerializer
    {
        byte[] Serialize(IMessage message);
    }
}
