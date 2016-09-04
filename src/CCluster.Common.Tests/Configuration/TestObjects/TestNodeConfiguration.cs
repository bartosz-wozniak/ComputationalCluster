using CommandLine;

namespace CCluster.Common.Tests.Configuration.TestObjects
{
    public class TestNodeConfiguration
    {
        [Option("address", Required = true)]
        public string CsAddress { get; set; }
        [Option("port", Required = true)]
        public int? CsPort { get; set; }
    }
}
