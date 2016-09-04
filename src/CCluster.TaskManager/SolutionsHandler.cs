using System;
using System.Threading.Tasks;
using CCluster.Common;
using CCluster.Common.Communication;
using CCluster.Common.Solver;
using CCluster.Messages;
using log4net;
using MediatR;

namespace CCluster.TaskManager
{
    public class SolutionsHandler : RequestHandler<Solutions>
    {
        private readonly ILog logger = LogProvider.GetCurrentClassLogger();

        private readonly Func<IQueuedServerClient> serverClientFactory;
        private readonly TaskSolverFactory taskSolverFactory;

        public SolutionsHandler(Func<IQueuedServerClient> serverClientFactory, TaskSolverFactory taskSolverFactory)
        {
            this.serverClientFactory = serverClientFactory;
            this.taskSolverFactory = taskSolverFactory;
        }

        protected override void HandleCore(Solutions message)
        {
            logger.Info($"Received order to merge problems for task {message.Id}.");
            Task.Factory.StartNew(() =>
            {
                var taskSolver = taskSolverFactory.GetTaskSolver(Constants.ProblemName, message.CommonData);
                byte[][] data = new byte[message.SolutionsList.Length][];
                for (int i = 0; i < message.SolutionsList.Length; i++)
                {
                    data[i] = message.SolutionsList[i].Data;
                }

                var answer = taskSolver.MergeSolution(data);
                var answerData = findSolutionForData(answer, message); //check how to get data from final solution (computationstime, taskid, etc.)
                var solution = new Solutions
                {
                    Id = message.Id,
                    ProblemType = message.ProblemType,
                    CommonData = message.CommonData,
                    SolutionsList = new[]
                    {
                        new Solution
                        {//TODO change data!!!!!!
                            ComputationsTime = message.SolutionsList[0].ComputationsTime,
                            Data = answer,
                            TimeoutOccured =  message.SolutionsList[0].TimeoutOccured,
                            TaskId =  message.SolutionsList[0].TaskId,
                            Type = SolutionType.Final
                        }
                    }
                };
                var client = serverClientFactory();
                client.Send(solution);
            });
        }

        private Solution findSolutionForData(byte[] data, Solutions message)
        {
            for (int i = 0; i < message.SolutionsList.Length; i++)
            {
                if (message.SolutionsList[i].Equals(data))
                {
                    return message.SolutionsList[i];
                }
            }

            return null;
        }
    }
}
