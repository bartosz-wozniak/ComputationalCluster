using CommandLine;

namespace CCluster.Common.Configuration
{
    public class NodeConfiguration
    {
        [Option("address", Required = false)]
        public string CsAddress { get; set; }
        [Option("port", Required = false)]
        public int CsPort { get; set; }
    }
}
