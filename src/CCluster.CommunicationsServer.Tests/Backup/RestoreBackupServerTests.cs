using System.Net;
using CCluster.Common.Configuration;
using CCluster.CommunicationsServer.Backup;
using CCluster.CommunicationsServer.Notifications;
using CCluster.Messages.Notifications;
using FakeItEasy;
using MediatR;

namespace CCluster.CommunicationsServer.Tests.Backup
{
    public class RestoreBackupServerTests
    {
        private static readonly BackupNodeInfo BackupServer = new BackupNodeInfo(2, IPAddress.Broadcast, 78);

        private readonly ICsConfigurationProvider cfgProvider;
        private readonly IBackupServerManager backupServers;
        private readonly IMediator mediator;

        private readonly RestoreBackupServer handler;

        public RestoreBackupServerTests()
        {
            cfgProvider = A.Fake<ICsConfigurationProvider>();
            backupServers = A.Fake<IBackupServerManager>();
            mediator = A.Fake<IMediator>();

            handler = new RestoreBackupServer(cfgProvider, backupServers, mediator);
        }

        public void Changes_CS_configuration_when_primary_server_is_available()
        {
            A.CallTo(() => backupServers.GetNextPrimaryServer()).Returns(BackupServer);

            handler.Handle(new ServerStoppedResponding());

            A.CallTo(() => cfgProvider.ChangeConfiguration(BackupServer.Address, BackupServer.Port))
                .MustHaveHappened();
        }

        public void Publishes_SwitchdToPrimary_notification_when_next_primary_server_is_unavailable()
        {
            A.CallTo(() => backupServers.GetNextPrimaryServer()).Returns(null);

            handler.Handle(new ServerStoppedResponding());

            A.CallTo(() => mediator.Publish(A<SwitchedToPrimary>.Ignored))
                .MustHaveHappened(Repeated.Exactly.Once);
        }
    }
}
