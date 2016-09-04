using System;
using System.Net;
using CCluster.Messages;
using MediatR;

namespace CCluster.CommunicationsServer.Messaging
{
    public class BackupClientMessage : IRequest
    {
        public IMessage Message { get; }
        public IPEndPoint MessageSource { get; }

        public BackupClientMessage(IMessage message, IPEndPoint messageSource)
        {
            this.Message = message;
            this.MessageSource = messageSource;
        }

        // Hacky, but unfortunately this is the only way as far as I can see
        public static INotification Create(BackupClientMessage msg)
        {
            var type = typeof(BackupClientMessage<>).MakeGenericType(msg.Message.GetType());
            return (INotification)Activator.CreateInstance(type, msg.Message, msg.MessageSource);
        }
    }

    public class BackupClientMessage<TMessage> : INotification
        where TMessage : IMessage
    {
        public TMessage Message { get; }
        public IPEndPoint MessageSource { get; }

        public BackupClientMessage(TMessage message, IPEndPoint messageSource)
        {
            this.Message = message;
            this.MessageSource = messageSource;
        }
    }
}