using System;
using System.Collections.Generic;
using log4net;
using UCCTaskSolver;

namespace CCluster.Common.Solver
{
    public class TaskSolverFactory
    {
        private readonly List<Type> taskSolverTypes = new List<Type>();
        private readonly ILog logger = LogProvider.GetCurrentClassLogger();
        private readonly TaskSolverTypesLoader taskSolverTypesLoader;

        public TaskSolverFactory(TaskSolverTypesLoader taskSolverTypesLoader)
        {
            this.taskSolverTypesLoader = taskSolverTypesLoader;
        }

        public void LoadTaskSolvers(string directory)
        {
            taskSolverTypes.AddRange(taskSolverTypesLoader.LoadPluginTypes(directory));
        }

        public TaskSolver GetTaskSolver(string name, byte[] data)
        {
            Type result = taskSolverTypes.Find(type => type.Name.Equals(name));
            if (result != null)
            {
                return (TaskSolver) Activator.CreateInstance(result, data);
            }

            logger.Fatal("Cannot find TaskSolver for specified name!");

            return null;
        } 
    }
}