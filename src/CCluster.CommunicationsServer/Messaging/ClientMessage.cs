using System;
using CCluster.Common.Communication;
using CCluster.Common.Communication.Messaging;
using CCluster.Messages;
using MediatR;

namespace CCluster.CommunicationsServer.Messaging
{
    public class ClientMessage<TMessage> : IRequest
        where TMessage : IMessage
    {
        public TMessage Message { get; }
        public ITcpClient Client { get; }

        public ClientMessage(TMessage message, ITcpClient client)
        {
            this.Message = message;
            this.Client = client;
        }

        public void Respond(IMessagesSender sender, params IMessage[] response)
        {
            using (Client)
            {
                sender.Send(response, Client.GetStream());
            }
        }

        public void Respond(IMessagesSender sender, IMessage response)
        {
            using (Client)
            {
                sender.Send(response, Client.GetStream());
            }
        }
    }

    public static class ClientMessage
    {
        // Hacky, but unfortunately this is the only way as far as I can see
        public static IRequest Create(IMessage msg, ITcpClient client)
        {
            var type = typeof(ClientMessage<>).MakeGenericType(msg.GetType());
            return (IRequest)Activator.CreateInstance(type, msg, client);
        }
    }
}
