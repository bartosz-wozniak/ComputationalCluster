using CCluster.Common;
using CCluster.Common.Configuration;
using CCluster.CommunicationsServer.Notifications;
using CCluster.Messages.Notifications;
using log4net;
using MediatR;

namespace CCluster.CommunicationsServer.Backup
{
    public class RestoreBackupServer : INotificationHandler<ServerStoppedResponding>
    {
        private readonly ILog logger = LogProvider.GetCurrentClassLogger();

        private readonly ICsConfigurationProvider cfgProvider;
        private readonly IBackupServerManager backupManager;
        private readonly IMediator mediator;

        public RestoreBackupServer(ICsConfigurationProvider cfgProvider, IBackupServerManager backupManager,
            IMediator mediator)
        {
            this.backupManager = backupManager;
            this.cfgProvider = cfgProvider;
            this.mediator = mediator;
        }

        public void Handle(ServerStoppedResponding notification)
        {
            var server = backupManager.GetNextPrimaryServer();
            if (server != null)
            {
                logger.Error($"Primary CS is down, restoring connection to {server.Address}:{server.Port}.");
                cfgProvider.ChangeConfiguration(server.Address, server.Port);
                mediator.Publish(new ConnectionRestored());
            }
            else
            {
                logger.Error("Primary CS is down, notifying system we need to work as primary");
                mediator.Publish(new SwitchedToPrimary());
            }
        }
    }
}
