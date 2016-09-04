using System.Collections.Generic;
using System.Reflection;
using Autofac;
using Autofac.Features.Variance;
using MediatR;

using Module = Autofac.Module;

namespace CCluster.Common
{
    public class MediatRModule : Module
    {
        private readonly Assembly sourceAssembly;

        public MediatRModule(Assembly sourceAssembly)
        {
            this.sourceAssembly = sourceAssembly;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterSource(new ContravariantRegistrationSource());
            builder.RegisterAssemblyTypes(typeof(IMediator).Assembly)
                .Where(t => t != typeof(Mediator))
                .AsImplementedInterfaces();

            builder.RegisterType<EventBus.SequentialMediator>()
                .AsImplementedInterfaces().SingleInstance()
                .AsSelf().SingleInstance();

            builder.RegisterAssemblyTypes(sourceAssembly)
                .AsClosedTypesOf(typeof(IRequestHandler<,>))
                .AsSelf().AsImplementedInterfaces();

            builder.RegisterAssemblyTypes(sourceAssembly)
                .AsClosedTypesOf(typeof(IAsyncRequestHandler<,>))
                .AsSelf().AsImplementedInterfaces();

            builder.RegisterAssemblyTypes(sourceAssembly)
                .AsClosedTypesOf(typeof(INotificationHandler<>))
                .AsSelf().AsImplementedInterfaces();

            builder.RegisterAssemblyTypes(sourceAssembly)
                .AsClosedTypesOf(typeof(IAsyncNotificationHandler<>))
                .AsSelf().AsImplementedInterfaces();

            builder.Register<SingleInstanceFactory>(ctx =>
            {
                var c = ctx.Resolve<IComponentContext>();
                return t => c.Resolve(t);
            });
            builder.Register<MultiInstanceFactory>(ctx =>
            {
                var c = ctx.Resolve<IComponentContext>();
                return t => (IEnumerable<object>)c.Resolve(typeof(IEnumerable<>).MakeGenericType(t));
            });
        }
    }
}
