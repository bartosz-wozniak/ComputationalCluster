using MediatR;

namespace CCluster.Messages
{
    public sealed class StatusMessage : IMessage, INotification
    {
        public ulong Id { get; set; }
    }
}
