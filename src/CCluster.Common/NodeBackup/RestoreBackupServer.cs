using System.Threading.Tasks;
using CCluster.Common.Configuration;
using CCluster.Messages.Notifications;
using log4net;
using MediatR;

namespace CCluster.Common.NodeBackup
{
    public class RestoreBackupServer : INotificationHandler<ServerStoppedResponding>
    {
        private readonly ILog logger = LogProvider.GetCurrentClassLogger();

        private readonly ICsConfigurationProvider cfgProvider;
        private readonly IBackupManager backupManager;
        private readonly IMediator mediator;

        public RestoreBackupServer(ICsConfigurationProvider cfgProvider, IBackupManager backupManager,
            IMediator mediator)
        {
            this.backupManager = backupManager;
            this.cfgProvider = cfgProvider;
            this.mediator = mediator;
        }

        public void Handle(ServerStoppedResponding notification)
        {
            // TODO: should we wait a little bit more just to let CS start its work?
            var server = backupManager.GetNextServer();
            if (server != null)
            {
                logger.Error($"Primary CS is down, restoring connection to {server.Address}:{server.Port}.");
                cfgProvider.ChangeConfiguration(server.Address, server.Port);

                Task.Factory.StartNew(async () =>
                {
                    await Task.Delay(2000);
                    mediator.Publish(new ConnectionRestored());
                });
            }
            else
            {
                logger.Error("Primary CS is down but we don't have other backup servers, forcing exit");
                mediator.Send(new ShutdownSystem());
            }
        }
    }
}
