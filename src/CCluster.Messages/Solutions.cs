namespace CCluster.Messages
{
    public enum SolutionType
    {
        Ongoing,
        Partial,
        Final
    }

    public class Solutions : IMessage
    {
        public string ProblemType { get; set; }
        public ulong Id { get; set; }
        public byte[] CommonData { get; set; }
        public Solution[] SolutionsList { get; set; }
    }

    public class Solution
    {
        public ulong TaskId { get; set; }
        public bool TimeoutOccured { get; set; }
        public SolutionType Type { get; set; }
        public ulong ComputationsTime { get; set; }
        public byte[] Data { get; set; }

        // Not in docs!
        public ulong NodeID { get; set; }
    }
}
