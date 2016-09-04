using CCluster.Common.Communication.Messaging;
using CCluster.CommunicationsServer.Backup;
using CCluster.CommunicationsServer.Messaging;
using CCluster.Messages;
using MediatR;

namespace CCluster.CommunicationsServer.ProblemManagement.Handlers
{
    public abstract class BaseProblemHandler<TMessage> : RequestHandler<ClientMessage<TMessage>>,
        INotificationHandler<BackupClientMessage<TMessage>>
        where TMessage : IMessage
    {
        protected readonly IProblemManager manager;
        protected readonly IMessagesSender sender;
        private readonly IBackupSender backupSender;

        public BaseProblemHandler(IProblemManager manager, IMessagesSender sender, IBackupSender backupSender)
        {
            this.manager = manager;
            this.sender = sender;
            this.backupSender = backupSender;
        }

        protected override void HandleCore(ClientMessage<TMessage> message)
        {
            var response = HandlePrimary(message.Message);
            message.Respond(sender, response);

            backupSender.Send(message.Message);
        }

        public void Handle(BackupClientMessage<TMessage> notification)
        {
            HandleBackup(notification.Message);
        }

        protected abstract IMessage HandlePrimary(TMessage message);
        protected virtual void HandleBackup(TMessage message)
        {
            HandlePrimary(message);
        }
    }
}
