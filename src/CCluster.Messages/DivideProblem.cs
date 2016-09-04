namespace CCluster.Messages
{
    public class DivideProblem : IMessage
    {
        public string ProblemType { get; set; }
        public ulong Id { get; set; }
        public byte[] Data { get; set; }
        public ulong ComputationalNodes { get; set; }
        public ulong NodeID { get; set; }
    }
}
