using System.Collections.Generic;

namespace CCluster.Messages
{
    public enum ProblemState
    {
        WaitingForDivision,
        Dividing,
        PartialProblemsSolving,
        WaitingForMerge,
        Merging,
        Completed
    }

    public class ProblemSync : IMessage
    {
        public ulong Id { get; set; }
        public string Type { get; set; }
        public byte[] Data { get; set; }

        public ProblemState State { get; set; }
        public SubProblemSync[] SubProblems { get; set; }

        public ulong? AssignedNode { get; set; }
    }

    public class SubProblemSync
    {
        public ulong Id { get; set; }
        public byte[] Data { get; set; }

        public bool IsFinished { get; set; }
        public byte[] Result { get; set; }
        public ulong? AssignedNode { get; set; }
    }
}
