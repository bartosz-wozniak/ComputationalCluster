using System.Collections.Generic;
using System.Linq;
using System.Net;
using CCluster.Common;

namespace CCluster.CommunicationsServer.NodeTrack
{
    public class CsDataStore : ICsDataStore
    {
        private readonly object lockObj = new object();

        private readonly List<NodeInfo> connectedNodes = new List<NodeInfo>();
        private readonly ITimeProvider timeProvider;

        public IReadOnlyList<NodeInfo> ConnectedNodes
        {
            get
            {
                lock (lockObj)
                {
                    return connectedNodes.ToArray();
                }
            }
        }

        public CsDataStore(ITimeProvider timeProvider)
        {
            this.timeProvider = timeProvider;
        }

        public void AddNode(ulong id, string type, IReadOnlyList<string> supportedProblems)
        {
            lock (lockObj)
            {
                if (!connectedNodes.Any(n => n.Id == id))
                {
                    connectedNodes.Add(new NodeInfo(id, type, supportedProblems)
                    {
                        LastStatusMessageTime = timeProvider.Now()
                    });
                }
            }
        }

        public NodeInfo GetById(ulong id)
        {
            lock (lockObj)
            { 
                return connectedNodes.FirstOrDefault(n => n.Id == id);
            }
        }

        public void RemoveNode(NodeInfo ni)
        {
            lock (lockObj)
            {
                connectedNodes.Remove(ni);
            }
        }
    }
}
