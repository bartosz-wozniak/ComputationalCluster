using CommandLine;

namespace CCluster.Common.Tests.Configuration.TestObjects
{
    public class TestServerConfiguration
    {
        [Option("port")]
        public int? ListeningPortNumber { get; set; }

        [Option("backup")]
        public bool IsBackup { get; set; }

        [Option("mport")]
        public int? MasterServerListeningPortNumber { get; set; }

        [Option("maddress")]
        public string MasterServerAddress { get; set; }

        [Option('t')]
        public int? CommunicationsTimeout { get; set; }
    }
}
