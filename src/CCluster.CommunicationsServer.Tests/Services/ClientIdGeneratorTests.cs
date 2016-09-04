using System.Net;
using CCluster.CommunicationsServer.Messaging;
using CCluster.CommunicationsServer.Services;
using CCluster.Messages.Register;
using Shouldly;

namespace CCluster.CommunicationsServer.Tests.Services
{
    public class ClientIdGeneratorTests
    {
        private readonly ClientIdGenerator generator;

        public ClientIdGeneratorTests()
        {
            generator = new ClientIdGenerator();
        }

        public void Returns_increasing_sequence()
        {
            var ids = new[] { generator.Next(), generator.Next(), generator.Next(), generator.Next() };

            for (int i = 1; i < ids.Length; i++)
            {
                ids[i].ShouldBeGreaterThan(ids[i - 1]);
            }
        }

        public void When_RegisterMessage_is_received_The_internal_counter_accomodates_newly_assigned_id()
        {
            const ulong Id = 789;
            var msg = new BackupClientMessage<RegisterMessage>(new RegisterMessage { Id = Id },
                new IPEndPoint(IPAddress.Loopback, 78));

            generator.Handle(msg);

            var id = generator.Next();
            id.ShouldBeGreaterThan(Id);
        }
    }
}
