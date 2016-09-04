using System.Net;
using CCluster.Common.Communication;
using CCluster.Common.Communication.Messaging;
using CCluster.CommunicationsServer.Backup;
using CCluster.CommunicationsServer.Handlers;
using CCluster.CommunicationsServer.Messaging;
using CCluster.CommunicationsServer.ProblemManagement;
using CCluster.Messages;
using FakeItEasy;
using MediatR;
using Shouldly;

namespace CCluster.CommunicationsServer.Tests.Handlers
{
    public class StatusMessageHandlerTests
    {
        const ulong Id = 789;
        private static readonly BackupNodeInfo[] SampleServers = new[]
        {
            new BackupNodeInfo(123, IPAddress.Loopback, 456),
            new BackupNodeInfo(789, IPAddress.Loopback, 123)
        };

        private readonly IMediator mediator;
        private readonly IMessagesSender sender;
        private readonly IBackupServerManager backupServers;
        private readonly ITcpClient client;
        private readonly IProblemDispatcher dispatcher;
        private readonly IBackupSender backupSender;

        private readonly StatusMessageHandler handler;

        public StatusMessageHandlerTests()
        {
            mediator = A.Fake<IMediator>();
            sender = A.Fake<IMessagesSender>();
            backupServers = A.Fake<IBackupServerManager>();
            client = A.Fake<ITcpClient>();
            dispatcher = A.Fake<IProblemDispatcher>();
            backupSender = A.Fake<IBackupSender>();

            A.CallTo(() => client.GetStream()).Returns(A.Fake<INetworkStream>());
            A.CallTo(() => backupServers.BackupServers).Returns(SampleServers);
            A.CallTo(() => dispatcher.GetWorkForNode(0)).WithAnyArguments().Returns(null);

            handler = new StatusMessageHandler(sender, mediator, backupServers, dispatcher, backupSender);
        }

        public void Republishes_the_status_message_as_notification()
        {
            var msg = Handle();

            A.CallTo(() => mediator.Publish(msg)).MustHaveHappened(Repeated.Exactly.Once);
        }

        public void Accesses_the_backup_servers_only_once()
        {
            Handle();

            A.CallTo(() => backupServers.BackupServers).MustHaveHappened(Repeated.Exactly.Once);
        }

        public void Disposes_the_TcpClient()
        {
            Handle();

            A.CallTo(() => client.Dispose()).MustHaveHappened(Repeated.Exactly.Once);
        }

        public void Sends_response_to_the_client_only()
        {
            Handle();

            A.CallTo(() => sender.Send(A<IMessage>.Ignored, client.GetStream()))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        public void Sends_only_NoOperation_message()
        {
            Handle();

            A.CallTo(() => sender.Send(A<NoOperation>.Ignored, A<INetworkStream>.Ignored))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        public void The_NoOperation_message_contains_all_backup_servers()
        {
            IMessage msg = null;
            A.CallTo(() => sender.Send((IMessage)null, null)).WithAnyArguments()
                .Invokes((IMessage m, INetworkStream _) => msg = m);

            Handle();

            var noop = msg.ShouldBeOfType<NoOperation>();
            foreach (var srv in SampleServers)
            {
                noop.BackupServers.ShouldContain(s =>
                    s.Id == srv.Id &&
                    s.Address == srv.Address.ToString() &&
                    s.Port == srv.Port);
            }
        }

        private StatusMessage Handle()
        {
            var msg = new StatusMessage { Id = Id };
            var clientMsg = new ClientMessage<StatusMessage>(msg, client);
            handler.Handle(clientMsg);
            return msg;
        }
    }
}
