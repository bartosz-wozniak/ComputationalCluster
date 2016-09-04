using System.Linq;
using System.Net;
using CCluster.Common;
using CCluster.Common.Communication;
using CCluster.Common.Communication.Messaging;
using CCluster.CommunicationsServer.Backup;
using CCluster.CommunicationsServer.Handlers;
using CCluster.CommunicationsServer.Messaging;
using CCluster.CommunicationsServer.Notifications;
using CCluster.CommunicationsServer.Services;
using CCluster.Messages.Register;
using FakeItEasy;
using MediatR;

namespace CCluster.CommunicationsServer.Tests.Handlers
{
    public class RegisterMessageHandlerTests
    {
        private const uint Timeout = 1234;
        private const ulong NodeId = 456;

        private static readonly IPEndPoint Endpoint = new IPEndPoint(IPAddress.Loopback, 4567);

        private readonly IMessagesSender sender;
        private readonly CommunicationsServerConfiguration config;
        private readonly IClientIdGenerator idGenerator;
        private readonly IBackupServerManager backupManager;
        private readonly IMediator mediator;
        private readonly ITcpClient client;
        private readonly IBackupSender backupSender;

        private readonly RegisterMessage message;

        private readonly RegisterMessageHandler handler;

        public RegisterMessageHandlerTests()
        {
            sender = A.Fake<IMessagesSender>();
            config = new CommunicationsServerConfiguration { CommunicationsTimeout = Timeout };
            idGenerator = A.Fake<IClientIdGenerator>();
            backupManager = A.Fake<IBackupServerManager>();
            mediator = A.Fake<IMediator>();
            client = A.Fake<ITcpClient>();
            backupSender = A.Fake<IBackupSender>();
            A.CallTo(() => backupManager.BackupServers).Returns(A.CollectionOfFake<BackupNodeInfo>(0).ToArray());
            A.CallTo(() => client.GetStream()).Returns(A.Fake<INetworkStream>());
            A.CallTo(() => client.RemoteEndpoint).Returns(Endpoint);

            message = new RegisterMessage { Type = Constants.NodeTypes.ComputationalNode };

            A.CallTo(() => idGenerator.Next()).Returns(NodeId);

            handler = new RegisterMessageHandler(sender, config, idGenerator, mediator, backupSender);
        }

        public void Generates_new_id()
        {
            Handle();

            A.CallTo(() => idGenerator.Next()).MustHaveHappened(Repeated.Exactly.Once);
        }

        public void Publishes_a_notification_about_new_node()
        {
            Handle();

            A.CallTo(() => mediator.Publish(
                A<NodeRegistered>.That.Matches(n => n.MessageSource == Endpoint && n.Message == message))
            ).MustHaveHappened(Repeated.Exactly.Once);
        }

        public void Sends_response_with_newly_generated_id_and_server_configuration()
        {
            Handle();

            A.CallTo(() =>
                sender.Send(A<RegisterResponse>.That.Matches(r =>
                r.Id == NodeId && r.Timeout == Timeout
                ), client.GetStream())
            ).MustHaveHappened(Repeated.Exactly.Once);
        }

        public void Resends_the_message_to_backup_server()
        {
            Handle();

            A.CallTo(() => backupSender.Send(A<RegisterResponse>.That.Matches(r =>
                 r.Id == NodeId && r.Timeout == Timeout)));
        }

        public void Publishes_NodeDead_notification_when_sending_deregister_message()
        {
            message.Id = NodeId;
            message.Deregister = true;
            Handle();

            A.CallTo(() => mediator.Publish(A<NodeDead>.That.Matches(m => m.Id == NodeId)));
        }

        private void Handle()
        {
            handler.Handle(new ClientMessage<RegisterMessage>(message, client));
        }
    }
}
