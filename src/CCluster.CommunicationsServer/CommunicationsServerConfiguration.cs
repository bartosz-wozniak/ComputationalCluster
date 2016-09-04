using System;
using System.Net;
using CCluster.Common.Communication;
using CommandLine;

namespace CCluster.CommunicationsServer
{
    public class CommunicationsServerConfiguration
    {
        [Option("port")]
        public int Port { get; set; }
        [Option("backup")]
        public bool IsBackup { get; set; }
        [Option("mport")]
        public int MasterServerPort { get; set; }
        [Option("maddress")]
        public string MasterServerAddress { get; set; }
        [Option('t')]
        public uint CommunicationsTimeout { get; set; }

        public TimeSpan CommunicationsTimeoutTimeSpan => TimeSpan.FromSeconds(CommunicationsTimeout);
    }
}
