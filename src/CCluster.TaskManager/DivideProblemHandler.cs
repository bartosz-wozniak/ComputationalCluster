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

namespace CCluster.TaskManager
{
    public class DivideProblemHandler : RequestHandler<DivideProblem>
    {
        private readonly ILog logger = LogProvider.GetCurrentClassLogger();

        private readonly Func<IQueuedServerClient> serverClientFactory;
        private readonly TaskSolverFactory taskSolverFactory;

        public DivideProblemHandler(Func<IQueuedServerClient> serverClientFactory, TaskSolverFactory taskSolverFactory)
        {
            this.serverClientFactory = serverClientFactory;
            this.taskSolverFactory = taskSolverFactory;
        }

        protected override void HandleCore(DivideProblem message)
        {
            logger.Info($"Received order to divide problems for task {message.Id}.");
            Task.Factory.StartNew(() =>
            {
                var taskSolver = taskSolverFactory.GetTaskSolver(Constants.ProblemName, message.Data);
                
                var data = taskSolver.DivideProblem(5);
                List<Solution> solutions = new List<Solution>();
                for (int i = 0; i < data.GetLength(0); i++)
                {
                    solutions.Add(new Solution()
                    {
                        TaskId = (ulong)i,
                        NodeID = message.NodeID,
                        Type = SolutionType.Ongoing,
                        Data = data[i]
                    });
                }

                var problems = new Solutions
                {
                    Id = message.Id,
                    ProblemType = message.ProblemType,
                    CommonData = message.Data,
                    SolutionsList = solutions.ToArray()
                };
                var client = serverClientFactory();
                client.Send(problems);
            });
        }
    }
}
