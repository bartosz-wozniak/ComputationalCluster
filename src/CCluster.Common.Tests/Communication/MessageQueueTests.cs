using CCluster.Common.Communication;
using CCluster.Messages;
using FakeItEasy;

namespace CCluster.Common.Tests.Communication
{
    public class MessageQueueTests
    {
        private readonly IQueuedServerClient client;

        private readonly MessageQueue queue;

        public MessageQueueTests()
        {
            client = A.Fake<IQueuedServerClient>();

            queue = new MessageQueue(() => client);
        }

        public void Enqueuing_message_does_not_call_inner_client()
        {
            queue.Enqueue(A.Fake<IMessage>());

            A.CallTo(() => client.Send(null)).WithAnyArguments().MustNotHaveHappened();
        }

        public void Messages_should_be_sent_during_ConnectionRestored_notification()
        {
            var msgs = A.CollectionOfFake<IMessage>(5);

            foreach (var msg in msgs)
            {
                queue.Enqueue(msg);
            }

            queue.Handle(new Messages.Notifications.ConnectionRestored());

            foreach (var msg in msgs)
            {
                A.CallTo(() => client.Send(msg)).MustHaveHappened(Repeated.Exactly.Once);
            }
        }
    }
}
