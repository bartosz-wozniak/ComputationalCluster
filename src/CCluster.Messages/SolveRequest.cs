namespace CCluster.Messages
{
    public class SolveRequest : IMessage
    {
        public string ProblemType { get; set; }
        public ulong? SolvingTimeout { get; set; }
        public ulong Id { get; set; }
        public byte[] Data { get; set; }
    }
}
