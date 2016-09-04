using System;
using System.Collections.Generic;

namespace CCluster.CommunicationsServer.NodeTrack
{
    public class NodeInfo
    {
        public ulong Id { get; }
        public string Type { get; }
        public IReadOnlyList<string> SupportedProblems { get; }

        public DateTime LastStatusMessageTime { get; set; }

        public NodeInfo(ulong id, string type, IReadOnlyList<string> supportedProblems)
        {
            Id = id;
            Type = type;
            SupportedProblems = supportedProblems ?? new string[0];
        }
    }
}
