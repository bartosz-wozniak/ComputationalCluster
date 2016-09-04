using System;
using System.Collections.Generic;
using CCluster.Messages;
using CCluster.Messages.Notifications;
using MediatR;

namespace CCluster.Common.Communication
{
    public class MessageQueue : IMessageQueue, INotificationHandler<ConnectionRestored>
    {
        private readonly Func<IQueuedServerClient> clientFactory;
        private readonly List<IMessage> messages = new List<IMessage>();

        private readonly object lockObject = new object();

        public MessageQueue(Func<IQueuedServerClient> baseClient)
        {
            this.clientFactory = baseClient;
        }

        public void Enqueue(IMessage msg)
        {
            lock (lockObject)
            {
                messages.Add(msg);
            }
        }

        public void Handle(ConnectionRestored notification)
        {
            IMessage[] msgsCopy;
            lock (lockObject)
            {
                msgsCopy = messages.ToArray();
                messages.Clear();
            }

            var client = clientFactory();
            foreach (var msg in msgsCopy)
            {
                client.Send(msg);
            }
        }
    }
}