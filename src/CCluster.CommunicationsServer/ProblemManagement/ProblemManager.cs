using System;
using System.Collections.Generic;
using System.Linq;
using CCluster.Common;
using CCluster.Messages;
using log4net;

namespace CCluster.CommunicationsServer.ProblemManagement
{
    public class ProblemManager : IProblemManager
    {
        private readonly ILog logger = LogProvider.GetCurrentClassLogger();

        private readonly object lockObj = new object();

        private ulong counter = 0;
        private readonly Dictionary<ulong, Problem> problems = new Dictionary<ulong, Problem>();

        public IReadOnlyList<Problem> GetProblemsForSync()
        {
            lock (lockObj)
            {
                return problems.Values.ToList();
            }
        }

        public ulong AddProblem(ProblemDefinition def)
        {
            lock (lockObj)
            {
                var id = counter++;
                logger.Info($"Adding new problem of type {def.Type} and id {id}.");
                problems.Add(id, new Problem(id, def));
                return id;
            }
        }

        public void AddProblem(ProblemDefinition def, ulong forcedId)
        {
            lock (lockObj)
            {
                // TODO: detect why we have two messages in the queue!
                if (!problems.ContainsKey(forcedId))
                {
                    logger.Info($"Adding new problem of type {def.Type} and id {forcedId}.");
                    problems.Add(forcedId, new Problem(forcedId, def));
                    counter = Math.Max(counter, forcedId + 1);
                }
            }
        }

        public void AddProblem(ProblemSync message)
        {
            lock (lockObj)
            {
                if (!problems.ContainsKey(message.Id))
                {
                    logger.Info($"Adding new problem of type {message.Type} and id {message.Id}.");
                    problems.Add(message.Id, new Problem(message));
                    counter = Math.Max(counter, message.Id + 1);
                }
            }
        }

        public Problem GetProblemForDivision(string type, ulong nodeId)
        {
            return WithProblem(
                t => t.Type == type && t.State == ProblemState.WaitingForDivision,
                t => { t.State = ProblemState.Dividing; t.AssignedNode = nodeId; });
        }

        public Problem GetProblemForMerging(string type, ulong nodeId)
        {
            return WithProblem(
                t => t.Type == type && t.State == ProblemState.WaitingForMerge,
                t => { t.State = ProblemState.Merging; t.AssignedNode = nodeId; });
        }

        public Tuple<Problem, IReadOnlyList<SubProblem>> GetSubProblemsForSolving(string type, int max, ulong nodeId)
        {
            lock (lockObj)
            {
                var selectedProblems = problems.Values.Where(t => t.Type == type && t.State == ProblemState.PartialProblemsSolving);
                foreach (var problem in selectedProblems)
                {
                    var subs = problem.SubProblems.Where(t => t.AssignedNode == null && !t.IsFinished).Take(max).ToList();
                    if (subs.Count > 0)
                    {
                        foreach (var s in subs)
                        {
                            s.AssignedNode = nodeId;
                        }
                        return Tuple.Create(problem, (IReadOnlyList<SubProblem>)subs);
                    }
                }
                return null;
            }
        }

        public Problem GetProblemForSending(ulong problemId)
        {
            lock (lockObj)
            {
                return GetProblemInternal(problemId);
            }
        }

        public void MarkAsDivided(ulong problemId, IEnumerable<SubProblemDefinition> defs)
        {
            logger.Info($"Marking problem {problemId} as divided.");
            WithProblem(problemId, t =>
            {
                t.State = ProblemState.PartialProblemsSolving;
                t.AssignedNode = null;
                t.SubProblems = defs.Select(d => new SubProblem(d)).ToArray();
            });
        }

        public void MarkSubProblemsAsSolved(ulong problemId, IEnumerable<SubProblemSolution> solutions)
        {
            logger.Info($"Marking subproblems of {problemId} as solved.");
            WithProblem(problemId, t =>
            {
                foreach (var solution in solutions)
                {
                    var subProblem = t.SubProblems.FirstOrDefault(p => p.Id == solution.Id);
                    if (subProblem != null)
                    {
                        subProblem.AssignedNode = null;
                        subProblem.IsFinished = true;
                        subProblem.Result = solution.Data;
                    }
                }
                if (t.SubProblems.All(s => s.IsFinished))
                {
                    t.State = ProblemState.WaitingForMerge;
                }
            });
        }

        public void MarkAsCompleted(ulong problemId, byte[] finalData)
        {
            logger.Info($"Marking problem {problemId} completed.");
            WithProblem(problemId, t =>
            {
                t.State = ProblemState.Completed;
                t.AssignedNode = null;
                t.SubProblems = new[] { new SubProblem(finalData) };
            });
        }

        public void MarkAsSent(ulong problemId)
        {
            lock (lockObj)
            {
                logger.Info($"Info about task {problemId} sent, removing it.");
                var problem = GetProblemInternal(problemId);
                if (problem?.State == ProblemState.Completed)
                {
                    problems.Remove(problemId);
                }
            }
        }

        public void MarkAsDuringMerge(ulong problemId, ulong nodeId)
        {
            logger.Info($"Marking problem {problemId} as during merge in node {nodeId}.");
            WithProblem(problemId, t =>
            {
                t.State = ProblemState.Merging;
                t.AssignedNode = nodeId;
            });
        }

        public void MarkAsDuringDivide(ulong problemId, ulong nodeId)
        {
            logger.Info($"Marking problem {problemId} as during division in node {nodeId}.");
            WithProblem(problemId, t =>
            {
                t.State = ProblemState.Dividing;
                t.AssignedNode = nodeId;
            });
        }

        public void MarkAsDuringPartialProblemSolving(ulong problemId, ulong nodeId, IEnumerable<ulong> tasks)
        {
            logger.Info($"Marking problem {problemId} as during partial solving in {nodeId}.");
            var tasksSet = new HashSet<ulong>(tasks);
            WithProblem(problemId, t =>
            {
                foreach (var sub in t.SubProblems.Where(s => tasksSet.Contains(s.Id)))
                {
                    sub.AssignedNode = nodeId;
                }
            });
        }

        public void ComputationalNodeIsDead(ulong nodeId)
        {
            logger.Info($"Marking subtasks assigned to node {nodeId} as not-in-progress as the node is dead.");
            lock (lockObj)
            {
                var subProblems = problems.Values
                    .SelectMany(s => s.SubProblems)
                    .Where(s => s.AssignedNode == nodeId);
                foreach (var s in subProblems)
                {
                    s.AssignedNode = null;
                }
            }
        }
        public void TaskManagerIsDead(ulong nodeId)
        {
            logger.Info($"Marking merge/divide tasks assigned to node {nodeId} as not-in-progress as the node is dead.");
            lock (lockObj)
            {
                var assignedProblems = problems.Values
                    .Where(p => p.AssignedNode == nodeId);
                foreach (var problem in assignedProblems)
                {
                    problem.AssignedNode = null;
                    if (problem.State == ProblemState.Merging)
                    {
                        problem.State = ProblemState.WaitingForMerge;
                    }
                    else if (problem.State == ProblemState.Dividing)
                    {
                        problem.State = ProblemState.WaitingForDivision;
                    }
                }
            }
        }

        private Problem WithProblem(Func<Problem, bool> selector, Action<Problem> modifier)
        {
            lock (lockObj)
            {
                var problem = problems.Values.FirstOrDefault(selector);
                if (problem != null)
                {
                    modifier(problem);
                }
                return problem;
            }
        }

        private Problem WithProblem(ulong problemId, Action<Problem> modifier)
        {
            lock (lockObj)
            {
                var problem = GetProblemInternal(problemId);
                if (problem != null)
                {
                    modifier(problem);
                }
                return problem;
            }
        }

        private Problem GetProblemInternal(ulong problemId)
        {
            Problem result;
            problems.TryGetValue(problemId, out result);
            return result;
        }
    }
}
