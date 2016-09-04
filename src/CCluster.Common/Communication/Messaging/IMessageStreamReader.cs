using System.Collections.Generic;
using CCluster.Messages;

namespace CCluster.Common.Communication.Messaging
{
    /// <summary>
    /// Parses a list of messages sent using <see cref="IMessagesSender"/>.
    /// </summary>
    /// <remarks>
    /// It should operate on a <see cref="System.IO.Stream"/> that is bound to the instance.
    /// </remarks>
    public interface IMessageStreamReader
    {
        /// <summary>
        /// Indicates whether the stream may still contain messages and a call to <see cref="ReadAvailable"/> is
        /// required.
        /// </summary>
        bool MayHaveMessages { get; }

        /// <summary>
        /// Tries to parse available data. Returns empty list if not enough data is present.
        /// </summary>
        /// <returns></returns>
        IReadOnlyList<IMessage> ReadAvailable();
    }
}
