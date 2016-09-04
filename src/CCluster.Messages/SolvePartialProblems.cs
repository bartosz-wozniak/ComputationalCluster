namespace CCluster.Messages
{
    public class SolvePartialProblems : IMessage
    {
        public string ProblemType { get; set; }
        public ulong Id { get; set; }
        public byte[] CommonData { get; set; }
        public PartialProblem[] PartialProblems { get; set; }
    }

    public class PartialProblem
    {
        public ulong TaskId { get; set; }
        public byte[] Data { get; set; }
        public ulong NodeID { get; set; }
    }
}
