using System;
using System.Linq;
using CCluster.Common;
using CCluster.CommunicationsServer.NodeTrack;
using CCluster.Messages;
using log4net;

namespace CCluster.CommunicationsServer.ProblemManagement
{
    public class ProblemDispatcher : IProblemDispatcher
    {
        private readonly ILog logger = LogProvider.GetCurrentClassLogger();
        
        private readonly ICsDataStore nodeStore;
        private readonly IProblemManager manager;

        public ProblemDispatcher(ICsDataStore nodeStore, IProblemManager manager)
        {
            this.nodeStore = nodeStore;
            this.manager = manager;
        }

        public IMessage GetWorkForNode(ulong nodeId)
        {
            var node = nodeStore.GetById(nodeId);
            if (node != null)
            {
                if (node.Type == Constants.NodeTypes.ComputationalNode)
                {
                    return GetWorkForComputationalNode(node);
                }
                else if (node.Type == Constants.NodeTypes.TaskManager)
                {
                    return GetWorkForTaskManager(node);
                }
            }
            return null;
        }

        private IMessage GetWorkForComputationalNode(NodeInfo node)
        {
            var nodeId = node.Id;
            var problems = SelectTaskFor(node, t => manager.GetSubProblemsForSolving(t, Constants.NumberOfProblemsSendByOnce, nodeId));
            if (problems != null)
            {
                logger.Info($"Assigning work on subproblems of {problems.Item1.Id} to {nodeId}.");
                return new SolvePartialProblems
                {
                    ProblemType = problems.Item1.Type,
                    Id = problems.Item1.Id,
                    CommonData = problems.Item1.Data,
                    PartialProblems = problems.Item2.Select(s => new PartialProblem
                    {
                        TaskId = s.Id,
                        Data = s.Data,
                        NodeID = nodeId
                    }).ToArray()
                };
            }
            return null;
        }

        private IMessage GetWorkForTaskManager(NodeInfo node)
        {
            var nodeId = node.Id;
            var mergeTask = SelectTaskFor(node, t => manager.GetProblemForMerging(t, nodeId));
            if (mergeTask != null)
            {
                logger.Info($"Assigning merge task of problem {mergeTask.Id} to {nodeId}.");
                return new Solutions
                {
                    Id = mergeTask.Id,
                    ProblemType = mergeTask.Type,
                    CommonData = mergeTask.Data,
                    SolutionsList = mergeTask.SubProblems.Select(s => new Solution
                    {
                        TaskId = s.Id,
                        Data = s.Result,
                        Type = SolutionType.Partial,
                        NodeID = nodeId
                    }).ToArray()
                };
            }

            var divideTask = SelectTaskFor(node, t => manager.GetProblemForDivision(t, nodeId));
            if (divideTask != null)
            {
                logger.Info($"Assigning division of problem {divideTask.Id} to {nodeId}.");
                return new DivideProblem
                {
                    Id = divideTask.Id,
                    ProblemType = divideTask.Type,
                    ComputationalNodes = ulong.MaxValue,
                    Data = divideTask.Data,
                    NodeID = nodeId
                };
            }
            return null;
        }

        private TResult SelectTaskFor<TResult>(NodeInfo node, Func<string, TResult> selector)
            where TResult : class
        {
            if (node != null)
            {
                foreach (var type in node.SupportedProblems)
                {
                    var result = selector(type);
                    if (result != null)
                    {
                        return result;
                    }
                }
            }
            return null;
        }
    }
}
