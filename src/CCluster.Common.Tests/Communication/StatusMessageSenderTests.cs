using System;
using CCluster.Common.Communication;
using CCluster.Common.Communication.Exceptions;
using CCluster.Common.Communication.Status;
using CCluster.Messages;
using CCluster.Messages.Notifications;
using FakeItEasy;
using MediatR;

namespace CCluster.Common.Tests.Communication
{
    public class StatusMessageSenderTests
    {
        private const int TimeoutInSeconds = 10;
        private static readonly DateTime StartDate = new DateTime(2016, 3, 13, 13, 32, 00);

        private readonly ITimeProvider time;
        private readonly IServerClient client;
        private readonly IMediator mediator;

        private readonly IStatusMessageSender sender;

        public StatusMessageSenderTests()
        {
            time = A.Fake<ITimeProvider>();
            client = A.Fake<IServerClient>();
            mediator = A.Fake<IMediator>();

            A.CallTo(() => time.Now()).Returns(StartDate);

            sender = new StatusMessageSender(time, client, mediator);
            sender.Timeout = TimeSpan.FromSeconds(TimeoutInSeconds);
        }

        public void Does_not_send_first_status_immediately()
        {
            sender.SendIfRequired();

            A.CallTo(() => client.Send(null)).WithAnyArguments().MustNotHaveHappened();
        }

        public void Sends_first_message_after_initial_gap()
        {
            ConfigureTime();

            sender.SendIfRequired();

            A.CallTo(() => client.Send(A<IMessage>.That.IsInstanceOf(typeof(StatusMessage))))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        public void Does_not_send_second_message_before_second_gap()
        {
            ConfigureTime();
            sender.SendIfRequired();
            ConfigureTime(1.5);
            sender.SendIfRequired();

            A.CallTo(() => client.Send(A<IMessage>.That.IsInstanceOf(typeof(StatusMessage))))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        public void Sends_second_message_after_two_gaps()
        {
            ConfigureTime();
            sender.SendIfRequired();
            ConfigureTime(2);

            sender.SendIfRequired();

            A.CallTo(() => client.Send(A<IMessage>.That.IsInstanceOf(typeof(StatusMessage))))
                .MustHaveHappened(Repeated.Exactly.Twice);
        }

        public void If_send_attempt_is_delayed_more_than_one_gap_It_does_not_try_to_send_more_messages()
        {
            ConfigureTime(3);

            sender.SendIfRequired();

            A.CallTo(() => client.Send(A<IMessage>.That.IsInstanceOf(typeof(StatusMessage))))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        public void When_component_cannot_connect_to_the_CS_It_should_publish_ServerStoppedResponding_notification()
        {
            ConfigureTime();
            A.CallTo(client).Throws<CannotSendMessageException>();

            sender.SendIfRequired();

            A.CallTo(() => mediator.Publish(A<INotification>.That.IsInstanceOf(typeof(ServerStoppedResponding))))
                .MustHaveHappened();
        }

        public void When_CS_does_not_send_response_It_should_publish_ServerStoppedResponding_notification()
        {
            ConfigureTime();
            A.CallTo(client).Throws<NoResponseException>();

            sender.SendIfRequired();

            A.CallTo(() => mediator.Publish(A<INotification>.That.IsInstanceOf(typeof(ServerStoppedResponding))))
                .MustHaveHappened();
        }

        private void ConfigureTime(double multiplier = 1.0)
        {
            A.CallTo(() => time.Now()).Returns(StartDate.AddSeconds(TimeoutInSeconds * multiplier));
        }
    }
}
