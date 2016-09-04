using System.Collections.Generic;
using System.Linq;
using CCluster.Messages;

namespace CCluster.CommunicationsServer.ProblemManagement
{
    public class ProblemDefinition
    {
        public string Type { get; }
        public byte[] Data { get; }

        public ProblemDefinition(string type, byte[] data)
        {
            Type = type;
            Data = data;
        }
    }

    public class Problem
    {
        public ulong Id { get; }
        public string Type { get; }
        public byte[] Data { get; }

        public ProblemState State { get; set; }
        public IReadOnlyList<SubProblem> SubProblems { get; set; } = new SubProblem[0];

        public ulong? AssignedNode { get; set; }

        public Problem(ulong id, ProblemDefinition def)
        {
            Id = id;
            Type = def.Type;
            Data = def.Data;
        }

        internal Problem(ProblemSync sync)
        {
            Id = sync.Id;
            Type = sync.Type;
            Data = sync.Data;
            State = sync.State;
            SubProblems = sync.SubProblems.Select(s => new SubProblem(s)).ToArray();
            AssignedNode = sync.AssignedNode;
        }
    }

    public class SubProblemDefinition
    {
        public ulong Id { get; }
        public byte[] Data { get; }

        public SubProblemDefinition(ulong id, byte[] data)
        {
            Id = id;
            Data = data;
        }
    }

    public class SubProblemSolution
    {
        public ulong Id { get; }
        public byte[] Data { get; }

        public SubProblemSolution(ulong id, byte[] data)
        {
            Id = id;
            Data = data;
        }
    }

    public class SubProblem
    {
        public ulong Id { get; }
        public byte[] Data { get; }

        public bool IsFinished { get; set; }
        public byte[] Result { get; set; }
        public ulong? AssignedNode { get; set; }

        public SubProblem(SubProblemDefinition def)
        {
            Id = def.Id;
            Data = def.Data;
        }

        public SubProblem(byte[] data)
        {
            Id = 0;
            IsFinished = true;
            Result = data;
        }

        internal SubProblem(SubProblemSync sync)
        {
            Id = sync.Id;
            Data = sync.Data;
            IsFinished = sync.IsFinished;
            Result = sync.Result;
            AssignedNode = sync.AssignedNode;
        }
    }
}
