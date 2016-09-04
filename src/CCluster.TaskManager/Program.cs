using Autofac;
using CCluster.Common;
using CCluster.Common.Communication;
using CCluster.Common.Configuration;
using CCluster.Common.Solver;
using CCluster.Messages.Register;

namespace CCluster.TaskManager
{
    public class Program : NodeBase<NodeConfiguration, Program>
    {
        private TaskSolverFactory taskSolverFactory;

        static void Main(string[] args)
        {
            new Program().Run();
        }

        private IServerClient serverClient;

        public Program()
            : base(Constants.NodeTypes.TaskManager)
        { }

        protected override string GetAddressFromConfig(NodeConfiguration cfg)
        {
            return cfg.CsAddress;
        }

        protected override int GetPortFromConfig(NodeConfiguration cfg)
        {
            return cfg.CsPort;
        }

        protected override void Inject(ILifetimeScope scope)
        {
            serverClient = scope.Resolve<IServerClient>();
            Logger.Info("Loading plugins.");
            taskSolverFactory = scope.Resolve<TaskSolverFactory>();
            taskSolverFactory.LoadTaskSolvers(libDirectory);
        }

        protected override void LoadModules(ContainerBuilder builder)
        {
            base.LoadModules(builder);
            builder.RegisterModule<SolverModule>();
        }

        protected override void StartSystem()
        {
            serverClient.Send(new RegisterMessage { Type = NodeType, SolvableProblems = new[] { Constants.ProblemName } });
        }

        protected override void StopSystem()
        { }
    }
}
