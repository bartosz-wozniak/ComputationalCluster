using System.Collections.Generic;
using CCluster.Messages;

namespace CCluster.Common.Communication.Messaging
{
    /// <summary>
    /// Sends a list of messages to the output stream.
    /// </summary>
    public interface IMessagesSender
    {
        void Send(IReadOnlyList<IMessage> messages, INetworkStream outputStream);
        void Send(IMessage message, INetworkStream outputStream);
    }
}
