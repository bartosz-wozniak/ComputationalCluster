using System;
using Autofac;
using CCluster.CommunicationsServer.Backup;
using CCluster.CommunicationsServer.Messaging;
using CCluster.CommunicationsServer.NodeTrack;
using CCluster.CommunicationsServer.ProblemManagement;
using CCluster.CommunicationsServer.Services;
using CCluster.CommunicationsServer.Storage;

namespace CCluster.CommunicationsServer
{
    public class CsModule : Module
    {
        private readonly Func<CommunicationsServerConfiguration> configProvider;

        public CsModule(Func<CommunicationsServerConfiguration> configProvider)
        {
            this.configProvider = configProvider;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<CsDataStore>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<NodeTracker>().AsImplementedInterfaces();
            builder.RegisterType<NodeTrackerManager>().AsSelf();

            builder.RegisterType<PrimaryInputMessageListener>().AsSelf();
            builder.RegisterType<BackupInputMessageListener>().AsSelf();
            builder.RegisterType<ServerMessageStreamReader>().AsSelf();

            builder.RegisterType<ClientIdGenerator>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<BackupServerManager>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<BackupSender>().AsImplementedInterfaces().SingleInstance();

            builder.RegisterType<ProblemManager>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<ProblemDispatcher>().AsImplementedInterfaces();

            builder.Register(_ => configProvider()).AsSelf();

            builder.RegisterType<CommunicationsServer>().AsSelf().SingleInstance();
            builder.RegisterType<CommunicationServerStorage>().AsSelf().SingleInstance();

            builder.RegisterType<BackupCommunicationsServer>().AsSelf()
                .AsImplementedInterfaces()
                .SingleInstance();

            base.Load(builder);
        }
    }
}
