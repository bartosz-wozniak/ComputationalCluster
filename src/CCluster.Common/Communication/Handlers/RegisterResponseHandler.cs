using CCluster.Messages.Notifications;
using CCluster.Messages.Register;
using log4net;
using MediatR;

namespace CCluster.Common.Communication.Handlers
{
    public class RegisterResponseHandler : RequestHandler<RegisterResponse>
    {
        private readonly ILog logger = LogProvider.GetCurrentClassLogger();

        private readonly IMediator mediator;

        public RegisterResponseHandler(IMediator mediator)
        {
            this.mediator = mediator;
        }

        protected override void HandleCore(RegisterResponse message)
        {
            logger.Info($"Registered with master CS, our id: {message.Id}");
            mediator.Publish(new Registered { AssignedId = message.Id, Timeout = message.Timeout });
        }
    }
}
