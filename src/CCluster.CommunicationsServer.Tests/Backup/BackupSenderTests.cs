using System.Linq;
using System.Net;
using System.Net.Sockets;
using CCluster.Common.Communication;
using CCluster.Common.Communication.Messaging;
using CCluster.CommunicationsServer.Backup;
using CCluster.CommunicationsServer.Notifications;
using CCluster.Messages;
using FakeItEasy;
using MediatR;

namespace CCluster.CommunicationsServer.Tests.Backup
{
    public class BackupSenderTests
    {
        private static readonly BackupNodeInfo[] BackupNodes = new[]
        {
            new BackupNodeInfo(1, IPAddress.Loopback, 123),
            new BackupNodeInfo(2, IPAddress.Loopback, 456)
        };

        private readonly IMessagesSender messagesSender;
        private readonly IBackupServerManager backupServers;
        private readonly ITcpClient client;
        private readonly IMediator mediator;

        private readonly IMessage message;

        private readonly BackupSender sender;

        public BackupSenderTests()
        {
            messagesSender = A.Fake<IMessagesSender>();
            backupServers = A.Fake<IBackupServerManager>();
            client = A.Fake<ITcpClient>();
            mediator = A.Fake<IMediator>();
            message = A.Fake<IMessage>();

            A.CallTo(() => backupServers.GetFollowingNodes()).Returns(BackupNodes);
            A.CallTo(() => client.GetStream()).Returns(A.Fake<INetworkStream>());

            sender = new BackupSender(messagesSender, backupServers, () => client, mediator);
        }

        public void Gets_the_following_nodes_only()
        {
            sender.Send(message);

            A.CallTo(() => backupServers.GetFollowingNodes()).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(backupServers)
                .Where(m => m.Method.Name != nameof(backupServers.GetFollowingNodes))
                .MustNotHaveHappened();
        }

        public void Correctly_sends_the_message()
        {
            sender.Send(message);

            var node = BackupNodes[0];
            var order = A.SequentialCallContext();
            A.CallTo(() => client.Connect(node.Address, node.Port))
                .MustHaveHappened(Repeated.Exactly.Once).InOrder(order);
            A.CallTo(() => messagesSender.Send(message, client.GetStream()))
                .MustHaveHappened(Repeated.Exactly.Once).InOrder(order);
            A.CallTo(() => client.Dispose())
                .MustHaveHappened(Repeated.Exactly.Once).InOrder(order);
        }

        public void Does_not_try_second_server_if_first_one_succeeded()
        {
            sender.Send(message);

            var node = BackupNodes[1];
            A.CallTo(() => client.Connect(node.Address, node.Port))
                .MustNotHaveHappened();
        }

        public void If_server_fails_Publishes_NodeDead_notification()
        {
            var node = BackupNodes[0];
            A.CallTo(() => client.Connect(node.Address, node.Port)).WithAnyArguments().Throws<SocketException>();

            sender.Send(message);

            A.CallTo(() => mediator.Publish(A<NodeDead>.That.Matches(n => n.Id == node.Id)));
        }

        public void If_server_fails_Tries_next_server()
        {
            var node = BackupNodes[0];
            A.CallTo(() => client.Connect(node.Address, node.Port)).WithAnyArguments().Throws<SocketException>();

            sender.Send(message);

            var nextNode = BackupNodes[1];
            A.CallTo(() => client.Connect(nextNode.Address, nextNode.Port))
                .MustHaveHappened(Repeated.Exactly.Once);
        }
    }
}
