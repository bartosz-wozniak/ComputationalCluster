using CommandLine;

namespace CCluster.Client
{
    public class ClientConfiguration
    {
        [Option("address", Required = false)]
        public string CsAddress { get; set; }
        [Option("port", Required = false)]
        public int CsPort { get; set; }

        [Option("problemId", Required = false)]
        public ulong? ProblemId { get; set; }

    }
}
