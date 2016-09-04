using Autofac;
using CCluster.Common.Communication;
using CCluster.Common.Communication.Handlers;
using CCluster.Common.Communication.Messaging;
using CCluster.Common.Communication.Status;

namespace CCluster.Common
{
    public class CommunicationModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<MessageQueue>().AsImplementedInterfaces().SingleInstance();

            builder.RegisterType<ServerClient>().AsImplementedInterfaces();
            builder.RegisterType<QueuedServerClient>().AsImplementedInterfaces();

            builder.RegisterType<TcpClientWrapper>().AsImplementedInterfaces();

            builder.RegisterType<RegisterResponseHandler>().AsImplementedInterfaces();

            builder.RegisterType<MessagesSender>().AsImplementedInterfaces();
            builder.RegisterType<MessageStreamReader>().AsImplementedInterfaces();
            builder.RegisterType<SimpleMessageSerializer>().AsImplementedInterfaces();

            builder.RegisterType<StatusMessageSender>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<StatusManager>().AsSelf().AsImplementedInterfaces().SingleInstance();

        }
    }
}
