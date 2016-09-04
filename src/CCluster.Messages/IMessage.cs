using MediatR;

namespace CCluster.Messages
{
    /// <summary>
    /// Marker interface for every message sent over the wire.
    /// </summary>
    public interface IMessage : IRequest
    { }

    /// <summary>
    /// Marker interface for every <see cref="IMessage"/> that does not need response from the other side.
    /// </summary>
    public interface INoResponseMessage : IMessage
    { }
}
