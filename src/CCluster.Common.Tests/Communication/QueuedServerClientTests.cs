using CCluster.Common.Communication;
using CCluster.Common.Communication.Exceptions;
using CCluster.Messages;
using FakeItEasy;
using Shouldly;

namespace CCluster.Common.Tests.Communication
{
    public class QueuedServerClientTests
    {
        private readonly IMessage Message = A.Fake<IMessage>();

        private readonly IServerClient baseClient;
        private readonly IMessageQueue queue;

        private readonly QueuedServerClient client;

        public QueuedServerClientTests()
        {
            baseClient = A.Fake<IServerClient>();
            queue = A.Fake<IMessageQueue>();

            client = new QueuedServerClient(baseClient, queue);
        }

        public void Calls_base_client()
        {
            client.Send(Message);

            A.CallTo(() => baseClient.Send(Message)).MustHaveHappened();
        }

        public void When_message_is_sent_correctly_Does_not_store_it_in_queue()
        {
            client.Send(Message);

            A.CallTo(() => queue.Enqueue(Message)).MustNotHaveHappened();
        }

        public void When_base_client_throws_CannotSendMessageException_The_exception_is_suppressed()
        {
            A.CallTo(() => baseClient.Send(Message)).Throws<CannotSendMessageException>();

            Should.NotThrow(() => client.Send(Message));
        }

        public void When_base_client_throws_NoResponseException_The_exception_is_suppressed()
        {
            A.CallTo(() => baseClient.Send(Message)).Throws<NoResponseException>();

            Should.NotThrow(() => client.Send(Message));
        }

        public void When_base_client_throws_CannotSendMessageException_The_message_is_stored_in_queue()
        {
            A.CallTo(() => baseClient.Send(Message)).Throws<CannotSendMessageException>();

            client.Send(Message);

            A.CallTo(() => queue.Enqueue(Message)).MustHaveHappened();
        }

        public void When_base_client_throws_NoResponseException_The_message_is_stored_in_queue()
        {
            A.CallTo(() => baseClient.Send(Message)).Throws<NoResponseException>();

            client.Send(Message);

            A.CallTo(() => queue.Enqueue(Message)).MustHaveHappened();
        }
    }
}