using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CCluster.Common;
using CCluster.Common.Communication;
using CCluster.Common.Solver;
using CCluster.Messages;
using log4net;
using MediatR;

namespace CCluster.ComputationalNode
{
    public class PartialProblemsHandler : RequestHandler<SolvePartialProblems>
    {
        private readonly ILog logger = LogProvider.GetCurrentClassLogger();

        private readonly Func<IQueuedServerClient> serverClientFactory;
        private readonly TaskSolverFactory taskSolverFactory;

        public PartialProblemsHandler(Func<IQueuedServerClient> serverClientFactory, TaskSolverFactory taskSolverFactory)
        {
            this.serverClientFactory = serverClientFactory;
            this.taskSolverFactory = taskSolverFactory;
        }

        protected override void HandleCore(SolvePartialProblems message)
        {
            logger.Info($"Received order to solve partial problems for task {message.Id}.");
            Task.Factory.StartNew(() =>
            {
                var client = serverClientFactory();

                var taskSolver = taskSolverFactory.GetTaskSolver(Constants.ProblemName, message.CommonData);
                List<Solution> solutions = new List<Solution>();
                for (int i = 0; i < message.PartialProblems.Length; i++)
                {
                    solutions.Add(
                        new Solution()
                        {
                            ComputationsTime = 10,
                            Data = taskSolver.Solve(message.PartialProblems[i].Data, new TimeSpan(0, 0, 2)),
                            TaskId = message.PartialProblems[i].TaskId,
                            TimeoutOccured = false,
                            Type = SolutionType.Final
                        });

                }
                client.Send(new Solutions()
                {
                    Id = message.Id,
                    CommonData = message.CommonData,
                    SolutionsList = solutions.ToArray()
                });
            });
        }
    }
}
