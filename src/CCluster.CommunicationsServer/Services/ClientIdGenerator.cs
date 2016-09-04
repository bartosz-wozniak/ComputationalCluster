using System;
using System.Threading;
using CCluster.CommunicationsServer.Messaging;
using CCluster.Messages.Register;
using MediatR;

namespace CCluster.CommunicationsServer.Services
{
    public class ClientIdGenerator : IClientIdGenerator, INotificationHandler<BackupClientMessage<RegisterMessage>>
    {
        private long counter = 0;

        public ulong Next()
        {
            return (ulong)Interlocked.Increment(ref counter);
        }

        public void Handle(BackupClientMessage<RegisterMessage> notification)
        {
            long org, newValue;
            do
            {
                org = counter;
                newValue = Math.Max(org, (long)notification.Message.Id + 1);
            } while (newValue != org && Interlocked.CompareExchange(ref counter, newValue, org) == newValue);
        }
    }
}
