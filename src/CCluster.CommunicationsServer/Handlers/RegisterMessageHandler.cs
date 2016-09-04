using CCluster.Common;
using CCluster.Common.Communication.Messaging;
using CCluster.CommunicationsServer.Backup;
using CCluster.CommunicationsServer.Messaging;
using CCluster.CommunicationsServer.Notifications;
using CCluster.CommunicationsServer.Services;
using CCluster.Messages.Register;
using log4net;
using MediatR;

namespace CCluster.CommunicationsServer.Handlers
{
    public class RegisterMessageHandler : RequestHandler<ClientMessage<RegisterMessage>>
    {
        private readonly ILog logger = LogProvider.GetCurrentClassLogger();

        private readonly IMessagesSender messagesSender;
        private readonly CommunicationsServerConfiguration config;
        private readonly IClientIdGenerator generator;
        private readonly IMediator mediator;
        private readonly IBackupSender sender;

        public RegisterMessageHandler(IMessagesSender messagesSender, CommunicationsServerConfiguration config,
            IClientIdGenerator generator, IMediator mediator, IBackupSender sender)
        {
            this.messagesSender = messagesSender;
            this.config = config;
            this.generator = generator;
            this.mediator = mediator;
            this.sender = sender;
        }

        protected override void HandleCore(ClientMessage<RegisterMessage> message)
        {
            if (message.Message.Deregister)
            {
                HandleDeregister(message.Message);
            }
            else
            {
                HandleRegister(message);
            }
        }

        private void HandleDeregister(RegisterMessage message)
        {
            logger.Info($"Node {message.Id} wants to deregister, marking as dead");
            mediator.Publish(new NodeDead(message.Id));
        }

        private void HandleRegister(ClientMessage<RegisterMessage> message)
        {
            logger.Info($"Registering node {message.Client} as a {message.Message.Type}");
            message.Message.Id = generator.Next();

            sender.Send(message.Message);

            mediator.Publish(new NodeRegistered(
                msg: message.Message,
                messageSource: message.Client.RemoteEndpoint));

            RegisterResponse responseMessage = new RegisterResponse
            {
                Id = message.Message.Id,
                Timeout = config.CommunicationsTimeout
            };
            message.Respond(messagesSender, responseMessage);
        }
    }
}