using MediatR;

namespace CCluster.Messages.Notifications
{
    public class Registered : INotification
    {
        public ulong AssignedId { get; set; }
        public uint Timeout { get; set; }
    }
}
