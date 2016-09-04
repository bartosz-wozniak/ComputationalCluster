using CCluster.Common.Communication.Handlers;
using CCluster.Messages.Notifications;
using CCluster.Messages.Register;
using FakeItEasy;
using MediatR;

namespace CCluster.Common.Tests.Communication.Handlers
{
    public class RegisterResponseHandlerTests
    {
        private readonly IMediator mediator;
        private readonly RegisterResponseHandler handler;

        public RegisterResponseHandlerTests()
        {
            mediator = A.Fake<IMediator>();
            handler = new RegisterResponseHandler(mediator);
        }

        public void Publishes_Registered_notification()
        {
            handler.Handle(new RegisterResponse());

            A.CallTo(() => mediator.Publish(A<INotification>.That.IsInstanceOf(typeof(Registered))))
                .MustHaveHappened();
        }

        public void The_notification_has_correct_data_copied_from_the_message()
        {
            const ulong Id = 123;
            const uint Timeout = 44;

            handler.Handle(new RegisterResponse { Id = Id, Timeout = Timeout });

            A.CallTo(() => mediator.Publish(A<Registered>.That.Matches(r =>
                r.AssignedId == Id && r.Timeout == Timeout
            )));
        }
    }
}
