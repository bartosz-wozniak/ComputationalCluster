using System.Collections.Generic;

namespace CCluster.CommunicationsServer.NodeTrack
{
    // TODO: decide how this interface should look like
    public interface ICsDataStore
    {
        IReadOnlyList<NodeInfo> ConnectedNodes { get; }

        void AddNode(ulong id, string type, IReadOnlyList<string> supportedProblems);
        NodeInfo GetById(ulong id);

        void RemoveNode(NodeInfo ni);
    }
}
