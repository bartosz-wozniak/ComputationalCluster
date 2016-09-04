using Autofac;
using CCluster.Common;

namespace CCluster.CommunicationsServer
{
    public class Program : NodeBase<CommunicationsServerConfiguration, Program>
    {
        static void Main(string[] args)
        {
            new Program().Run();
        }

        private IMainServer mainServer;

        public Program()
            : base(Constants.NodeTypes.CommunicationsServer)
        { }

        protected override void Inject(ILifetimeScope scope)
        {
            mainServer = Configuration.IsBackup ?
                scope.Resolve<BackupCommunicationsServer>() :
                (IMainServer)scope.Resolve<CommunicationsServer>();
        }

        protected override void StartSystem()
        {
            mainServer.Start();
        }

        protected override void StopSystem()
        {
            mainServer.Stop();
        }

        protected override string GetAddressFromConfig(CommunicationsServerConfiguration cfg)
        {
            return cfg.MasterServerAddress;
        }

        protected override int GetPortFromConfig(CommunicationsServerConfiguration cfg)
        {
            return cfg.MasterServerPort;
        }

        protected override void LoadModules(ContainerBuilder builder)
        {
            builder.RegisterModule<CommunicationModule>();
            builder.RegisterModule<ConfigurationModule>();
            builder.RegisterModule(new CsModule(() => Configuration));
        }
    }
}
