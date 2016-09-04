using System.Net;
using CCluster.Messages.Register;
using MediatR;

namespace CCluster.CommunicationsServer.Notifications
{
    public class NodeRegistered : INotification
    {
        public RegisterMessage Message { get; }
        public IPEndPoint MessageSource { get; }

        public NodeRegistered(RegisterMessage msg, IPEndPoint messageSource)
        {
            this.Message = msg;
            this.MessageSource = messageSource;
        }
    }
}
