using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using CCluster.Common;
using CCluster.CommunicationsServer.NodeTrack;
using CCluster.CommunicationsServer.Notifications;
using CCluster.Messages;
using CCluster.Messages.Register;
using FakeItEasy;
using MediatR;
using Shouldly;

namespace CCluster.CommunicationsServer.Tests.NodeTrack
{
    public class NodeTrackerTests
    {
        const uint Timeout = 45;
        private static readonly DateTime CurrentDate = new DateTime(2016, 3, 28, 20, 42, 0);

        private readonly ICsDataStore store;
        private readonly ITimeProvider time;
        private readonly IMediator mediator;
        private readonly CommunicationsServerConfiguration config;

        private readonly NodeTracker tracker;

        public NodeTrackerTests()
        {
            store = A.Fake<ICsDataStore>();
            time = A.Fake<ITimeProvider>();
            mediator = A.Fake<IMediator>();
            config = new CommunicationsServerConfiguration { CommunicationsTimeout = Timeout };

            A.CallTo(() => time.Now()).Returns(CurrentDate);

            tracker = new NodeTracker(store, time, mediator, config);
        }

        public void Updates_LastStatusMessageTime_on_StatusMessage()
        {
            var node = new NodeInfo(3, "test", new string[0]);
            A.CallTo(() => store.GetById(node.Id)).Returns(node);

            tracker.Handle(new StatusMessage { Id = node.Id });

            node.LastStatusMessageTime.ShouldBe(CurrentDate);
        }

        public void Does_not_touch_other_nodes_on_StatusMessage()
        {
            const ulong Id = 3;
            tracker.Handle(new StatusMessage { Id = Id });

            A.CallTo(() => store.GetById(A<ulong>.That.Matches(e => e != Id)))
                .MustNotHaveHappened();
        }

        public void Adds_node_to_store_on_NodeRegistered_message()
        {
            const ulong Id = 4;
            const string Type = "test";
            var msg = new NodeRegistered(new RegisterMessage { Id = Id, Type = Type },
                new IPEndPoint(IPAddress.Loopback, 123));
            tracker.Handle(msg);

            A.CallTo(() => store.AddNode(Id, Type, A<IReadOnlyList<string>>.Ignored))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        public void Resets_LastStatusMessageTime_on_SwitchedToPrimary()
        {
            var nodes = PrepareNodes();

            tracker.Handle(new SwitchedToPrimary());

            foreach (var node in nodes)
            {
                node.LastStatusMessageTime.ShouldBe(CurrentDate);
            }
        }

        public void Does_nothing_when_node_is_not_outdated()
        {
            PrepareNodes();

            tracker.DiscardOutdatedNodes();

            A.CallTo(() => store.RemoveNode(null)).WithAnyArguments().MustNotHaveHappened();
            A.CallTo(() => mediator.Publish(null)).WithAnyArguments().MustNotHaveHappened();
        }

        public void Removes_outdated_nodes()
        {
            var nodes = PrepareNodes(true);

            tracker.DiscardOutdatedNodes();

            A.CallTo(() => store.RemoveNode(nodes[0])).MustHaveHappened(Repeated.Exactly.Once);
        }

        public void Publishes_notification_about_node_removal()
        {
            var nodes = PrepareNodes(true);

            tracker.DiscardOutdatedNodes();

            A.CallTo(() => mediator.Publish(A<NodeRemoved>.That.Matches(n => n.Id == nodes[0].Id)))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        private NodeInfo[] PrepareNodes(bool firstOutdated = false)
        {
            var nodes = new[]
            {
                new NodeInfo(1, Constants.NodeTypes.CommunicationsServer, new string[0])
                {
                    LastStatusMessageTime = firstOutdated ? CurrentDate.AddSeconds(-Timeout - 1) : CurrentDate
                },
                new NodeInfo(2, Constants.NodeTypes.ComputationalNode, new string[0])
                {
                    LastStatusMessageTime = CurrentDate
                },
                new NodeInfo(3, Constants.NodeTypes.TaskManager, new string[0])
                {
                    LastStatusMessageTime = CurrentDate
                }
            };
            A.CallTo(() => store.ConnectedNodes).Returns(nodes);
            return nodes;
        }
    }
}
