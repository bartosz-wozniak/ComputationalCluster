using Autofac;
using CCluster.Common.Solver;

namespace CCluster.Common
{
    public class SolverModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<TaskSolverTypesLoader>().AsSelf();
            builder.RegisterType<TaskSolverFactory>().AsSelf().SingleInstance();
        }
    }
}