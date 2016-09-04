using System;
using System.Threading;
using CCluster.Common;
using CCluster.Messages.Notifications;
using log4net;
using MediatR;

namespace CCluster.Common.Communication.Status
{
    /// <summary>
    /// Manages a thread that periodically sends a Status message.
    /// </summary>
    public class StatusManager : SoftThread, INotificationHandler<Registered>,
        INotificationHandler<ServerStoppedResponding>, INotificationHandler<ConnectionRestored>
    {
        private readonly ILog logger = LogProvider.GetCurrentClassLogger();

        private readonly IStatusMessageSender sender;

        public StatusManager(IStatusMessageSender sender)
        {
            this.sender = sender;
        }

        public void Handle(Registered notification)
        {
            logger.Debug("Register notification received, preparing thread");
            sender.Timeout = TimeSpan.FromSeconds(notification.Timeout / 2);
            Start();
        }

        public void Handle(ServerStoppedResponding notification)
        {
            logger.Debug("Server stopped responding, aborting status thread");
            Stop();
        }

        public void Handle(ConnectionRestored notification)
        {
            logger.Debug("Connection to CS restored (may be backup), restarting thread");
            Start();
        }

        protected override void ThreadMain(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                token.WaitHandle.WaitOne(sender.Timeout);
                if (token.IsCancellationRequested)
                {
                    break;
                }
                sender.SendIfRequired();
            }
        }
    }
}
