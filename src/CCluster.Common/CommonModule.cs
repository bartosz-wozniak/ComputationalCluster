using Autofac;
using CCluster.Common.Configuration.Reader;

namespace CCluster.Common
{
    public class CommonModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<NetTimeProvider>().AsImplementedInterfaces();
        }
    }
}
