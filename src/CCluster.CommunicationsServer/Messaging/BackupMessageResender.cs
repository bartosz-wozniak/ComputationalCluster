using CCluster.Common;
using CCluster.CommunicationsServer.Backup;
using log4net;
using MediatR;

namespace CCluster.CommunicationsServer.Messaging
{
    public class BackupMessageResender : RequestHandler<BackupClientMessage>
    {
        private readonly ILog logger = LogProvider.GetCurrentClassLogger();

        private readonly IMediator mediator;
        private readonly IBackupSender backupSender;

        public BackupMessageResender(IMediator mediator, IBackupSender backupSender)
        {
            this.mediator = mediator;
            this.backupSender = backupSender;
        }

        protected override void HandleCore(BackupClientMessage message)
        {
            mediator.Publish(BackupClientMessage.Create(message));
            backupSender.Send(message.Message);
        }
    }
}