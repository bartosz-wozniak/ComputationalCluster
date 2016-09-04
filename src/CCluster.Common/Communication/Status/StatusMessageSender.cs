using System;
using CCluster.Common.Communication.Exceptions;
using CCluster.Messages;
using CCluster.Messages.Notifications;
using log4net;
using MediatR;

namespace CCluster.Common.Communication.Status
{
    /// <summary>
    /// TODO: update status message when notification is sent
    /// TODO: test the Registered notification !important
    /// </summary>
    public class StatusMessageSender : IStatusMessageSender, INotificationHandler<Registered>,
        INotificationHandler<ConnectionRestored>
    {
        private readonly ITimeProvider time;
        private readonly IServerClient client;
        private readonly IMediator mediator;
        private static readonly ILog log = LogProvider.GetCurrentClassLogger();
        private DateTime previousDate;

        private readonly StatusMessage messageTemplate = new StatusMessage();

        public TimeSpan Timeout { get; set; }

        public StatusMessageSender(ITimeProvider time, IServerClient client, IMediator mediator)
        {
            log.Info("Create StatusMessageSender");
            this.time = time;
            this.client = client;
            this.mediator = mediator;

            previousDate = time.Now();
        }

        public void SendIfRequired()
        {
            log.Info("Check if sending is required");
            var current = time.Now();
            if (current - previousDate >= Timeout)
            {
                previousDate = current;
                SendStatus();
            }
        }

        private void SendStatus()
        {
            log.Info("Send Status");
            try
            {
                client.Send(messageTemplate);
            }
            catch (CannotSendMessageException ex)
            {
                log.Error("CannotSendMessageException was caught: " + ex.Message);
                mediator.Publish(new ServerStoppedResponding());
            }
            catch (NoResponseException ex)
            {
                log.Error("NoResponseException was caught: " + ex.Message);
                mediator.Publish(new ServerStoppedResponding());
            }
        }

        public void Handle(Registered notification)
        {
            messageTemplate.Id = notification.AssignedId;
        }

        public void Handle(ConnectionRestored notification)
        {
            previousDate = time.Now();
        }
    }
}
