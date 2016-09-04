using System.Linq;
using CCluster.Common;
using CCluster.Common.Communication.Messaging;
using CCluster.CommunicationsServer.Backup;
using CCluster.CommunicationsServer.Messaging;
using CCluster.CommunicationsServer.ProblemManagement;
using CCluster.Messages;
using CCluster.Messages.Register;
using log4net;
using MediatR;

namespace CCluster.CommunicationsServer.Handlers
{
    public class StatusMessageHandler : RequestHandler<ClientMessage<StatusMessage>>
    {
        private readonly ILog logger = LogProvider.GetCurrentClassLogger();
        private readonly IMediator mediator;
        private readonly IMessagesSender messagesSender;
        private readonly IBackupServerManager backupServers;
        private readonly IProblemDispatcher problemDispatcher;
        private readonly IBackupSender backupSender;

        public StatusMessageHandler(IMessagesSender messagesSender, IMediator mediator,
            IBackupServerManager backupServers, IProblemDispatcher problemDispatcher, IBackupSender backupSender)
        {
            this.messagesSender = messagesSender;
            this.mediator = mediator;
            this.backupServers = backupServers;
            this.problemDispatcher = problemDispatcher;
            this.backupSender = backupSender;
        }

        protected override void HandleCore(ClientMessage<StatusMessage> message)
        {
            logger.Info($"Status message received from {message.Message.Id}");

            Respond(message);
            mediator.Publish(message.Message);
        }

        private void Respond(ClientMessage<StatusMessage> message)
        {
            var noop = GetNoOpMessage();
            var work = problemDispatcher.GetWorkForNode(message.Message.Id);

            if (work != null)
            {
                message.Respond(messagesSender, work, noop);
                backupSender.Send(work);
            }
            else
            {
                message.Respond(messagesSender, noop);
            }
        }

        private NoOperation GetNoOpMessage()
        {
            var servers = backupServers.BackupServers.Select(s =>
                new BackupServer
                {
                    Id = s.Id,
                    Address = s.Address.ToString(),
                    Port = s.Port
                })
                .ToArray();

            return new NoOperation { BackupServers = servers };
        }
    }
}
