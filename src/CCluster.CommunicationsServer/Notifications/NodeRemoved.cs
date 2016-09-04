using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCluster.CommunicationsServer.Notifications
{
    public class NodeRemoved : INotification
    {
        public ulong Id { get; }

        public NodeRemoved(ulong id)
        {
            this.Id = id;
        }
    }
}
