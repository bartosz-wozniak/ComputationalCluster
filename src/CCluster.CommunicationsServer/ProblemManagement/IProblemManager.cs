using System;
using System.Collections.Generic;
using CCluster.Messages;

namespace CCluster.CommunicationsServer.ProblemManagement
{
    public interface IProblemManager
    {
        IReadOnlyList<Problem> GetProblemsForSync();

        ulong AddProblem(ProblemDefinition problem);
        void AddProblem(ProblemDefinition problem, ulong forcedId);
        void AddProblem(ProblemSync message);

        Problem GetProblemForDivision(string type, ulong nodeId);
        Problem GetProblemForMerging(string type, ulong nodeId);
        Tuple<Problem, IReadOnlyList<SubProblem>> GetSubProblemsForSolving(string type, int max, ulong nodeId);
        Problem GetProblemForSending(ulong problemId);

        void MarkAsDivided(ulong problemId, IEnumerable<SubProblemDefinition> defs);
        void MarkSubProblemsAsSolved(ulong problemId, IEnumerable<SubProblemSolution> solutions);
        void MarkAsCompleted(ulong problemId, byte[] finalData);
        void MarkAsSent(ulong problemId);

        void MarkAsDuringMerge(ulong problemId, ulong nodeId);
        void MarkAsDuringDivide(ulong problemId, ulong nodeId);
        void MarkAsDuringPartialProblemSolving(ulong problemId, ulong nodeId, IEnumerable<ulong> tasks);

        void ComputationalNodeIsDead(ulong nodeId);
        void TaskManagerIsDead(ulong nodeId);
    }
}
