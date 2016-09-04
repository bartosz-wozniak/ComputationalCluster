using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using CCluster.Common;
using CCluster.Common.Communication;
using CCluster.Common.Communication.Messaging;
using CCluster.CommunicationsServer.Handlers;
using CCluster.CommunicationsServer.NodeTrack;
using CCluster.CommunicationsServer.Notifications;
using CCluster.CommunicationsServer.ProblemManagement;
using CCluster.CommunicationsServer.Storage;
using CCluster.Messages;
using CCluster.Messages.Register;
using FakeItEasy;
using MediatR;
using Shouldly;

namespace CCluster.CommunicationsServer.Tests.Handlers
{
    public class SendServerStateToBackupTests
    {
        private const int Id = 123;
        private static readonly IPAddress Address = IPAddress.Loopback;
        private const int Port = 7878;

        private readonly IMessagesSender sender;
        private readonly ICsDataStore dataStore;
        private readonly ITcpClient tcpClient;
        private readonly IMediator mediator;
        private readonly IProblemManager problemManager;
        private readonly CommunicationServerStorage communicationServerStorage;

        private readonly SendServerStateToBackup handler;

        public SendServerStateToBackupTests()
        {
            sender = A.Fake<IMessagesSender>();
            dataStore = A.Fake<ICsDataStore>();
            tcpClient = A.Fake<ITcpClient>();
            mediator = A.Fake<IMediator>();
            problemManager = A.Fake<IProblemManager>();
            communicationServerStorage = A.Fake<CommunicationServerStorage>();


            A.CallTo(() => dataStore.ConnectedNodes).Returns(new[] { new NodeInfo(123, "test", new string[0]) });

            handler = new SendServerStateToBackup(sender, () => tcpClient, dataStore, mediator, problemManager, communicationServerStorage);
        }

        public void Ignores_non_CS_nodes()
        {
            Handle(Constants.NodeTypes.ComputationalNode);

            A.CallTo(dataStore).MustNotHaveHappened();
            A.CallTo(sender).MustNotHaveHappened();
            A.CallTo(mediator).MustNotHaveHappened();
            A.CallTo(tcpClient).MustNotHaveHappened();
        }

        public void Ignores_Backup_CS()
        {
            communicationServerStorage.IsBackup = true;

            Handle();

            A.CallTo(dataStore).MustNotHaveHappened();
            A.CallTo(sender).MustNotHaveHappened();
            A.CallTo(mediator).MustNotHaveHappened();
            A.CallTo(tcpClient).MustNotHaveHappened();
        }

        public void Gets_the_nodes_from_data_store()
        {
            Handle();

            A.CallTo(() => dataStore.ConnectedNodes).MustHaveHappened(Repeated.Exactly.Once);
        }

        public void Sends_the_nodes_using_batched_messages()
        {
            Handle();

            A.CallTo(() => sender.Send((IReadOnlyList<IMessage>)null, null))
                .WithAnyArguments().MustHaveHappened(Repeated.Exactly.Once);
        }

        public void Maps_existing_nodes_to_RegisterMessage_and_sends_them()
        {
            var nodes = new[]
            {
                new NodeInfo(2, Constants.NodeTypes.CommunicationsServer, new string[0]),
                new NodeInfo(3, Constants.NodeTypes.TaskManager, new string[0])
            };
            A.CallTo(() => dataStore.ConnectedNodes).Returns(nodes);

            IReadOnlyList<IMessage> messages = null;
            A.CallTo(() => sender.Send((IReadOnlyList<IMessage>)null, null))
                .WithAnyArguments()
                .Invokes((IReadOnlyList<IMessage> a, INetworkStream _) => messages = a);

            Handle();

            messages.ShouldAllBe(m => m is RegisterMessage);
            var casted = messages.Cast<RegisterMessage>();
            foreach (var node in nodes)
            {
                casted.ShouldContain(m => m.Id == node.Id && m.Type == node.Type);
            }
        }

        public void Connects_with_the_client()
        {
            Handle();

            A.CallTo(() => tcpClient.Connect(Address, Port)).MustHaveHappened(Repeated.Exactly.Once);
        }

        public void Sends_the_messages_to_newly_connected_server()
        {
            var stream = A.Fake<INetworkStream>();
            A.CallTo(() => tcpClient.GetStream()).Returns(stream);

            Handle();

            A.CallTo(() => sender.Send(A<IReadOnlyList<IMessage>>.Ignored, stream))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        public void Disposes_the_client()
        {
            Handle();

            A.CallTo(() => tcpClient.Dispose()).MustHaveHappened(Repeated.Exactly.Once);
        }

        public void Marks_the_node_as_dead_when_connection_cannot_be_established()
        {
            A.CallTo(() => tcpClient.Connect(null, 0)).WithAnyArguments().Throws<SocketException>();

            Handle();

            A.CallTo(() => mediator.Publish(A<NodeDead>.That.Matches(n => n.Id == Id)))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        public void Marks_the_node_as_dead_when_cannot_send_messages()
        {
            A.CallTo(() => sender.Send((IReadOnlyList<IMessage>)null, null)).WithAnyArguments()
                .Throws<SocketException>();

            Handle();

            A.CallTo(() => mediator.Publish(A<NodeDead>.That.Matches(n => n.Id == Id)))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        private void Handle(string nodeType = Constants.NodeTypes.CommunicationsServer)
        {
            handler.Handle(new Notifications.NodeRegistered(
                new RegisterMessage { Id = Id, Type = nodeType },
                new IPEndPoint(Address, Port)
                ));
        }
    }
}
