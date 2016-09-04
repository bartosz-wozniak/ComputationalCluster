using Autofac;
using CCluster.Common.NodeBackup;

namespace CCluster.Common
{
    public class NodeBackupModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<BackupManager>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<RestoreBackupServer>().AsImplementedInterfaces();

            builder.RegisterType<NoOperationHandler>().AsImplementedInterfaces();
        }
    }
}
