using CCluster.Messages;

namespace CCluster.Common.Communication
{
    /// <summary>
    /// Main interface used to communicate with CS.
    /// </summary>
    public interface IServerClient
    {
        /// <summary>
        /// Sends a message to the CS and (if needed) waits for response.
        /// The response will be put back to queue.
        /// </summary>
        /// <param name="msg"></param>
        /// <exception cref="Exceptions.NoResponseException">
        /// Thrown when the server closes the connection without sending a response.
        /// </exception>
        /// <exception cref="Exceptions.CannotSendMessageException">Thrown when the message cannot be sent.</exception>
        void Send(IMessage msg);
    }
}
