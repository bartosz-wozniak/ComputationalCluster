using CCluster.Common.Communication.Exceptions;
using CCluster.Messages;

namespace CCluster.Common.Communication.Messaging
{
    /// <summary>
    /// Deserializes a single message.
    /// </summary>
    public interface IMessageDeserializer
    {
        /// <exception cref="CannotDeserializeMessageException">
        /// Thrown when specified data cannot be deserialized.
        /// </exception>
        IMessage Deserialize(byte[] data, int offset, int count);
    }
}
