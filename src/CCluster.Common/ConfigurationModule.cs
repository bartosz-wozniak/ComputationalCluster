using Autofac;
using CCluster.Common.Configuration;
using CCluster.Common.Configuration.Reader;

namespace CCluster.Common
{
    public class ConfigurationModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ContentReader>().AsImplementedInterfaces();
            builder.RegisterType<ConfigurationProvider>().AsImplementedInterfaces();
            builder.RegisterType<Loader>().AsSelf();

            builder.RegisterType<CsConfigurationProvider>().AsImplementedInterfaces().SingleInstance();
            builder.Register(ctx => ctx.Resolve<ICsConfigurationProvider>().Configuration);
        }
    }
}
